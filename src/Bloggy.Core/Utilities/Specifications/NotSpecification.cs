using System.Linq.Expressions;

namespace Bloggy.Core.Utilities.Specifications;

public class NotSpecification<T>(Specification<T> left) : Specification<T>
{
    private readonly Specification<T> _left = left;

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();

        var notExpression = Expression.Not(leftExpression.Body);

        return Expression.Lambda<Func<T, bool>>(notExpression, leftExpression.Parameters.Single());
    }
}
