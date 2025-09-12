namespace Bloggy.Core.Extensions;

public static class EnumExtensions
{
    public static int GetMaxLength(this Enum value)
    {
        return Enum.GetNames(value.GetType()).Max(x => x.Length);
    }

    public static int GetEnumMaxLength(this Type type)
    {
        return Enum.GetNames(type).Append(string.Empty).Max(x => x.Length);
    }
}