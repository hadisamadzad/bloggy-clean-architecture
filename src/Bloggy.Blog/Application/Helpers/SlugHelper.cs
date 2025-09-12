using System.Text;

namespace Bloggy.Blog.Application.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Tag name cannot be null or empty.", nameof(text));

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Replace special characters with specific words
        text = ReplaceSpecialCharacters(text);

        // Replace spaces with hyphens
        text = text.Replace(" ", "-");

        // Remove any remaining special characters
        text = RemoveRemainingSpecialCharacters(text);

        return text;
    }

    public static bool IsValidSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        // Check for special characters
        foreach (char @char in text)
            if (!char.IsLetterOrDigit(@char) && @char != '-')
                return false;

        // Check for consecutive hyphens, starting or ending hyphens
        if (text.Contains("--") || text.StartsWith('-') || text.EndsWith('-'))
            return false;

        return true;
    }

    private static string ReplaceSpecialCharacters(string input)
    {
        var replacements = new Dictionary<string, string>
        {
            { "#", "sharp" },
            { "&", "and" },
            { "@", "at" },
            { "+", "plus" },
        };
        foreach (var replacement in replacements)
        {
            input = input.Replace(replacement.Key, replacement.Value);
        }
        return input;
    }

    private static string RemoveRemainingSpecialCharacters(string input)
    {
        var sb = new StringBuilder();
        foreach (char c in input)
            if (char.IsLetterOrDigit(c) || c == '-')
                sb.Append(c);

        return sb.ToString();
    }
}