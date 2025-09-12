namespace Bloggy.Identity.Application.Helpers;

public static class ExpirationTimeHelper
{
    public static DateTime GetExpirationTime(int lifetimeInDays) => DateTime.UtcNow
        .AddDays(lifetimeInDays)
        .Date
        .Add(new TimeSpan(23, 59, 59));
}