using FluentValidation.Results;

namespace Bloggy.Core.Helpers;

public static class ValidationHelper
{
    public static IEnumerable<string> GetErrorMessages(this ValidationResult result)
    {
        return result.Errors.Select(x => x.ErrorMessage);
    }
}