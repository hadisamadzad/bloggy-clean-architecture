using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Bloggy.Core.Utilities.Specifications;

namespace Identity.Application.Specifications.Auth;

public partial class ValidUsernameSpecification : Specification<string>
{
    public ValidUsernameSpecification()
    {
    }

    public override Expression<Func<string, bool>> ToExpression()
    {
        return _ => MyRegex().Match(_).Success;
    }

    [GeneratedRegex("^(?=.{6,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$")]
    private static partial Regex MyRegex();
}