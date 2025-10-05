using Bloggy.Core.Interfaces;
using StackExchange.Redis;

namespace Bloggy.Core.Persistence.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    private readonly string Prefix;

    public RedisCacheService(IConnectionMultiplexer multiplexer, string instancePrefix)
    {
        if (string.IsNullOrWhiteSpace(instancePrefix))
            throw new ArgumentException("Invalid instance prefix. Provide a valid string as prefix");

        Prefix = $"{instancePrefix.Trim().Replace(" ", "")}";
        _connectionMultiplexer = multiplexer;
    }

    public async Task<T> GetAsync<T>(string key, bool hasAbsoluteKey = false)
    {
        var finalKey = hasAbsoluteKey ? key : $"{Prefix}-{key}";
        var value = await _connectionMultiplexer.GetDatabase().StringGetAsync(finalKey);

        if (value.IsNull)
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task<bool> SetAsync<T>(string key, T value)
    {
        var redisValue = JsonSerializer.Serialize(value);
        return await _connectionMultiplexer.GetDatabase().StringSetAsync($"{Prefix}-{key}", redisValue);
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var redisValue = JsonSerializer.Serialize(value);
        return await _connectionMultiplexer.GetDatabase().StringSetAsync($"{Prefix}-{key}", redisValue, ttl);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _connectionMultiplexer.GetDatabase().KeyDeleteAsync($"{Prefix}-{key}");
    }
}
