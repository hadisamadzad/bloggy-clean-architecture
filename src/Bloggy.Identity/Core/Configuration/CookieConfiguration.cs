namespace Bloggy.Identity.Core.Configuration;

public static class CookieConfiguration
{
    public static CookieOptions GetRefreshTokenOptions(TimeSpan expiry, bool isProduction = true)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Strict, // Can use Strict since same domain
            Domain = isProduction ? "bloggy.hadisamadzad.com" : null, // Exact domain, no subdomain sharing needed
            Expires = DateTimeOffset.UtcNow.Add(expiry),
            Path = "/",
            IsEssential = true
        };
    }
}