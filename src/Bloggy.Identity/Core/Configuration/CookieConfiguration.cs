namespace Bloggy.Identity.Core.Configuration;

public static class CookieConfiguration
{
    public static CookieOptions GetRefreshTokenOptions(TimeSpan expiry, bool isProduction = true)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.None, // Can use None since same domain
            Domain = null, // // Let browser handle it
            Expires = DateTimeOffset.UtcNow.Add(expiry),
            Path = "/",
            IsEssential = true
        };
    }
}