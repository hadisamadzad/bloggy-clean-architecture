namespace Bloggy.Identity.Api.Extensions;

public static class ResultExtensions
{
    public static IResult WithCookie(this IResult result, string name, string value, CookieOptions options)
    {
        return new CookieResult(result, name, value, options);
    }
}

internal class CookieResult(IResult result, string name, string value, CookieOptions options) : IResult
{
    private readonly IResult _result = result;
    private readonly string _name = name;
    private readonly string _value = value;
    private readonly CookieOptions _options = options;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Append(_name, _value, _options);
        await _result.ExecuteAsync(httpContext);
    }
}