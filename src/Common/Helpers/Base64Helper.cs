namespace Common.Helpers;

public static class Base64Helper
{
    public static string ConvertBase64ToBase64Url(string base64) =>
        base64.Replace('+', '-').Replace('/', '_');

    public static string ConvertBase64UrlToBase64(string base64Url) =>
        base64Url.Replace('-', '+').Replace('_', '/');
}