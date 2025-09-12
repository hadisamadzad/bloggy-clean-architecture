using HashidsNet;

namespace Bloggy.Core.Utilities;

public static class HashId
{
    private const string HashSalt = "***";
    private const int HashLength = 12;
    private const string HashAlphabets = "abcdefghklmnoprstuvw123456789";

    private static readonly Hashids Hasher = new(HashSalt, HashLength, HashAlphabets);

    public static string Encode(this int id)
    {
        return Hasher.Encode(id);
    }

    public static int Decode(this string eid)
    {
        try
        {
            return Hasher.Decode(eid)[0];
        }
        catch
        {
            return -1;
            throw new ArgumentException("Invalid encoded Id value");
        }
    }
}