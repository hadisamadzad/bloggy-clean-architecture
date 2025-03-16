using Common.Helpers;

namespace Identity.Application.Helpers;

public static class PasswordResetTokenHelper
{
    const string Prefix = "password-reset";
    const string _ = "|&|";

    const int PrefixIndex = 0;
    const int EmailIndex = 1;
    const int ExpirationIndex = 2;

    static string Key;

    public static void SetEncryptionKey(string key)
    {
        Key = key;
    }

    public static string GeneratePasswordResetToken(string email, DateTime expiration)
    {
        var randomPart = RandomGenerator.GenerateString(
            length: 4, allowedCharacters: AllowedCharacters.Alphanumeric);

        var payload = $"{Prefix}{_}{email}{_}{expiration:yyyy-MM-dd}{_}{randomPart}";
        var base64 = StringEncryptor.Encrypt(payload, Key);
        return Base64Helper.ConvertBase64ToBase64Url(base64);
    }

    public static (bool Succeeded, string Email) ReadPasswordResetToken(string passwordResetToken)
    {
        var base64 = Base64Helper.ConvertBase64UrlToBase64(passwordResetToken);
        var payload = StringEncryptor.Decrypt(base64, Key);
        var values = payload.Split(_);

        if (values[PrefixIndex] != Prefix)
            return (Succeeded: false, Email: string.Empty);

        if (DateTime.Parse(values[ExpirationIndex]) < DateTime.UtcNow)
            return (Succeeded: false, Email: string.Empty);

        return (Succeeded: true, Email: values[EmailIndex]);
    }
}