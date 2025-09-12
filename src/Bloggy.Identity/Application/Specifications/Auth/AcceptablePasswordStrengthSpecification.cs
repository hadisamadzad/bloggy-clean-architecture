using System.Linq.Expressions;
using Bloggy.Core.Utilities.Specifications;
using Identity.Application.Helpers;

namespace Bloggy.Identity.Application.Specifications.Auth;

public class AcceptablePasswordStrengthSpecification : Specification<string>
{
    public AcceptablePasswordStrengthSpecification()
    {
    }

    public override Expression<Func<string, bool>> ToExpression()
    {
        return _ => PasswordHelper.CheckStrength(_) >= PasswordScore.Medium;
    }
}