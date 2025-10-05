using Microsoft.Extensions.Caching.Distributed;

namespace Bloggy.Blog.Application.Helpers;

public static class CacheHelper
{
    public static DistributedCacheEntryOptions TtlFromSeconds(int value)
        => new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(value)
        };

    public static DistributedCacheEntryOptions TtlFromMinutes(int value)
        => new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(value)
        };

    public static DistributedCacheEntryOptions TtlFromHours(int value)
        => new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(value)
        };

    public static DistributedCacheEntryOptions TtlFromDays(int value)
        => new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(value)
        };
}