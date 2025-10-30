using Bloggy.Blog.Application.Interfaces;
using StackExchange.Redis;

namespace Bloggy.Blog.Infrastructure.Redis;

using Microsoft.Extensions.Logging;

public class ViewRedisRepository(IConnectionMultiplexer multiplexer, ILogger<ViewRedisRepository> logger) : IViewMemoryRepository
{
    private readonly IConnectionMultiplexer _multiplexer = multiplexer;
    private readonly ILogger<ViewRedisRepository> _logger = logger;

    public async Task<bool> AddUniqueViewAsync(string articleId, string visitorId)
    {
        var db = _multiplexer.GetDatabase();
        var seenKey = string.Format(RedisKeys.ViewsKey, articleId);
        var deltaKey = string.Format(RedisKeys.ViewsDeltaKey, articleId);

        // Add to the canonical "seen" set. If newly added, increment the per-article delta
        // which will be flushed by the background worker. This prevents losing seen-state
        // across flushes and makes repeated views by the same visitor within the retention
        // window counted only once.
        var added = await db.SetAddAsync(seenKey, visitorId);
        if (added)
        {
            // Increment delta atomically
            var delta = await db.StringIncrementAsync(deltaKey);
            // mark pending for flush
            await db.SetAddAsync(RedisKeys.PendingViewsKey, articleId);
            // ensure TTL is set so keys/deltas don't accumulate forever
            await db.KeyExpireAsync(seenKey, RedisKeys.ViewsTtl);
            await db.KeyExpireAsync(deltaKey, RedisKeys.ViewsTtl);

            _logger.LogDebug("New unique view for article {ArticleId} by visitor {VisitorId}; delta now {Delta}", articleId, visitorId, (long)delta);
        }
        else
        {
            _logger.LogTrace("Duplicate view ignored for article {ArticleId} by visitor {VisitorId}", articleId, visitorId);
        }

        return added;
    }

    public async Task<IEnumerable<string>> GetPendingArticleIdsAsync()
    {
        var db = _multiplexer.GetDatabase();
        var members = await db.SetMembersAsync(RedisKeys.PendingViewsKey);
        return members.Select(x => x.ToString());
    }

    public async Task<long> AtomicPopCountAsync(string articleId)
    {
        var db = _multiplexer.GetDatabase();
        var deltaKey = string.Format(RedisKeys.ViewsDeltaKey, articleId);

        // Atomically read-and-reset the per-article delta counter using GETSET.
        // This avoids using Lua and provides an atomic handoff: the returned value
        // is the number of new unique visitors since the last flush.
        var old = await db.StringGetSetAsync(deltaKey, "0");
        long count = 0;
        if (old.HasValue && long.TryParse(old.ToString(), out var parsed))
            count = parsed;

        if (count > 0)
            _logger.LogInformation("Flushing {Count} new unique views for article {ArticleId}", count, articleId);
        else
            _logger.LogDebug("No delta to flush for article {ArticleId}", articleId);

        // Remove from pending set regardless so we don't keep retrying empty entries.
        await db.SetRemoveAsync(RedisKeys.PendingViewsKey, articleId);

        return count;
    }
}
