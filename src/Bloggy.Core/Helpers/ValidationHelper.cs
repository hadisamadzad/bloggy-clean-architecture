using FluentValidation.Results;

namespace Bloggy.Core.Helpers;

public static class ValidationHelper
{
    public static string GetFirstErrorMessage(this ValidationResult result)
    {
        return result.Errors.FirstOrDefault()?.ErrorMessage;
    }

    public static object GetFirstError(this ValidationResult result)
    {
        return result.Errors.FirstOrDefault()?.CustomState;
    }
}