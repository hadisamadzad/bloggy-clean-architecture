using System.Security.Cryptography;
using System.Text;

namespace Common.Helpers;

public static class StringEncryptor
{
    public static string Encrypt(string plain, string key)
    {
        byte[] iv = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(iv); // Fill the byte array with random values

        byte[] fullCipher;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new(cryptoStream))
            {
                streamWriter.Write(plain);
            }

            var cipher = memoryStream.ToArray();

            // Combine IV and encrypted data
            fullCipher = new byte[iv.Length + cipher.Length];
            Array.Copy(iv, 0, fullCipher, 0, iv.Length);
            Array.Copy(cipher, 0, fullCipher, iv.Length, cipher.Length);
        }

        return Convert.ToBase64String(fullCipher);
    }

    public static string Decrypt(string cipher, string key)
    {
        var fullCipher = Convert.FromBase64String(cipher);

        // Extract the IV from the cipher text
        byte[] iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);

        // Extract the actual cipher text
        byte[] buffer = new byte[fullCipher.Length - iv.Length];
        Array.Copy(fullCipher, iv.Length, buffer, 0, buffer.Length);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
}
