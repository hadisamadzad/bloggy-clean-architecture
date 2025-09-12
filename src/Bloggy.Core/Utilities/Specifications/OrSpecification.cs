using System.Linq.Expressions;

namespace Bloggy.Core.Utilities.Specifications;

public class OrSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    private readonly Specification<T> _left = left;
    private readonly Specification<T> _right = right;

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var orExpression = Expression.OrElse(leftExpression.Body, rightExpression.Body);

        return Expression.Lambda<Func<T, bool>>(orExpression, leftExpression.Parameters.Single());
    }
}
