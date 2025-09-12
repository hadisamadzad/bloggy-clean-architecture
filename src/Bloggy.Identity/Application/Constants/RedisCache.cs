namespace Bloggy.Identity.Application.Constants;

public static class RedisCache
{
    public static string DevKey => "dev";
    public static int DevTtlInSeconds => 30;

    // Users
    public static string UserProfileKey => "userId-{0}-profile";
    public static int UserProfileTtlInMinutes => 30;
}