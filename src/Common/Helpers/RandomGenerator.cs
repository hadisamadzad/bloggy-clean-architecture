namespace Common.Helpers;

public static class AllowedCharacters
{
    public const string Numeric = "123456789";
    public const string Numeric0 = "1234567890";
    public const string Alphanumeric = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public const string AlphanumericCapital = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string AlphanumericReadable = "1234567890abcdefghkmnprstuvwyz";
}

public static class RandomGenerator
{
    readonly static Random randomGenerator = new();

    public static string GenerateString(int length, string allowedCharacters, string prefix = null)
    {
        var randomChars = new char[length];
        for (var i = 0; i < length; ++i)
            randomChars[i] = allowedCharacters[randomGenerator.Next(allowedCharacters.Length)];

        return $"{prefix}{new string(randomChars)}";
    }

    public static int GenerateNumber(int max, int min = 0)
    {
        if (max < min || max < 0 || min < 0)
            throw new ArgumentOutOfRangeException($"Invalid arguments: min={min} max={max}");
        return randomGenerator.Next(min, max);
    }
}