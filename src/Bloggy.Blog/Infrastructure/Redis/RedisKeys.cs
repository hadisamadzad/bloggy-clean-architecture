namespace Bloggy.Blog.Infrastructure.Redis;

public static class RedisKeys
{
    // Views
    public static string UserProfileKey => "userId-{0}-profile";
    public static int UserProfileTtlInMinutes => 30;

    private const string Prefix = "blog";
    public const string ViewsKey = Prefix + "-views-articleId:{0}"; // blog-views-articleId:{articleId}
    public const string PendingViewsKey = Prefix + "-views-pending-articleId:{0}"; // blog-views-pending-articleId:{articleId}
    public const string ViewsDeltaKey = Prefix + "-views-delta-articleId:{0}"; // blog-views-delta-articleId:{articleId}
    public static readonly TimeSpan ViewsTtl = TimeSpan.FromDays(7); // keep seen state for one week by default

}