using StackExchange.Redis;

namespace Bloggy.Core.Utilities.LockManager;

public class LockManager : ILockManager
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;

    private readonly string KeyPrefix;
    private const string KeySuffix = "lock";
    private const string LockValue = "Locked 🔒";

    public LockManager(ConfigurationOptions options, string instancePrefix)
    {
        if (string.IsNullOrWhiteSpace(instancePrefix))
            throw new ArgumentException($"Invalid instance prefix: {instancePrefix}.");

        KeyPrefix = instancePrefix.Trim().Replace(" ", "");
        _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
    }

    private RedisKey GenerateRedisKey(string key) => new($"{KeyPrefix}-{key}-{KeySuffix}");

    public async Task<bool> IsLockedAsync(string key)
    {
        var redisKey = GenerateRedisKey(key);
        return await _connectionMultiplexer.GetDatabase()
            .KeyExistsAsync(redisKey);
    }

    public async Task<LockResult> LockAsync(string key)
    {
        // Note: even if ttl is not provided we need to consider a retention policy for 1 day
        return await LockByTtlAsync(key, TimeSpan.FromDays(1));
    }

    public async Task<LockResult> LockByTtlAsync(string key, TimeSpan ttl, bool isSliding = false)
    {
        if (ttl.TotalSeconds < 1)
            throw new ArgumentException("Time to live should be greater than or equal to 1 second.");

        var redisKey = GenerateRedisKey(key);
        var database = _connectionMultiplexer.GetDatabase();
        // Get existing ttl if key already exists
        var existingTtl = await database.KeyTimeToLiveAsync(redisKey);
        if (existingTtl.HasValue)
        {
            if (isSliding)
                _ = await database.KeyExpireAsync(redisKey, ttl); // reset ttl is sliding is true
            return LockResult.AlreadyLocked;
        }

        // Insert the key if it's not existing
        var value = await database.StringGetSetAsync(redisKey, new RedisValue(LockValue));
        _ = await database.KeyExpireAsync(redisKey, ttl);
        if (value.HasValue)
            return LockResult.AlreadyLocked;

        return LockResult.SuccessfullyLocked;
    }

    public async Task<bool> UnlockAsync(string key)
    {
        var redisKey = GenerateRedisKey(key);
        return await _connectionMultiplexer.GetDatabase().KeyDeleteAsync(redisKey);
    }
}