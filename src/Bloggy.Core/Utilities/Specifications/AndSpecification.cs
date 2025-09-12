using System.Linq.Expressions;

namespace Bloggy.Core.Utilities.Specifications;

public class AndSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    private readonly Specification<T> _left = left;
    private readonly Specification<T> _right = right;

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var andExpression = Expression.AndAlso(leftExpression.Body, rightExpression.Body);

        return Expression.Lambda<Func<T, bool>>(andExpression, leftExpression.Parameters.Single());
    }
}
