namespace Bloggy.Core.Helpers;

public static class UidHelper
{
    public static string GenerateNewId(string prefix = default)
    {
        var ulid = Ulid.NewUlid().ToString().ToLower();
        return string.IsNullOrWhiteSpace(prefix) ? ulid : $"{prefix}-{ulid}";
    }
}