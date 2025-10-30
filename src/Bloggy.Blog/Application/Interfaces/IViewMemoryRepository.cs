namespace Bloggy.Blog.Application.Interfaces;

public interface IViewMemoryRepository
{
    /// <summary>
    /// Adds a unique visitor id for an article. Returns true if the visitor was newly added.
    /// </summary>
    Task<bool> AddUniqueViewAsync(string articleId, string visitorId);

    /// <summary>
    /// Returns the list of pending article ids which have new views to flush.
    /// </summary>
    Task<IEnumerable<string>> GetPendingArticleIdsAsync();

    /// <summary>
    /// Atomically move the set for the article into a temp key and return the cardinality (number of unique visitors)
    /// that were present, clearing the original set so new visitors are counted separately.
    /// </summary>
    Task<long> AtomicPopCountAsync(string articleId);
}
