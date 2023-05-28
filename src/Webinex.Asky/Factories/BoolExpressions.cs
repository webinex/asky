using System.Linq.Expressions;

namespace Webinex.Asky;

internal static class BoolExpressions
{
    public static Expression<Func<T, bool>> And<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        return GetAggregatedExpression(Expression.AndAlso, expr1, expr2);
    }

    public static Expression<Func<T, bool>> And<T>(
        Expression<Func<T, bool>>[] expressions)
    {
        if (expressions.Length < 2)
        {
            throw new ArgumentException("Might be at least 2 expressions", nameof(expressions));
        }

        return expressions.Skip(1).Aggregate(expressions.ElementAt(0), And);
    }

    public static Expression<Func<T, bool>> Or<T>(
        Expression<Func<T, bool>>[] expressions)
    {
        if (expressions.Length < 2)
        {
            throw new ArgumentException("Might be at least 2 expressions", nameof(expressions));
        }

        return expressions.Skip(1).Aggregate(expressions.ElementAt(0), Or);
    }

    public static Expression<Func<T, bool>> Or<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        return GetAggregatedExpression(Expression.OrElse, expr1, expr2);
    }

    private static Expression<Func<T, bool>> GetAggregatedExpression<T>(
        Func<Expression, Expression, BinaryExpression> aggregate,
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));
        var left = ParameterReplacer.Replace(expr1.Body, expr1.Parameters[0], parameter);
        var right = ParameterReplacer.Replace(expr2.Body, expr2.Parameters[0], parameter);

        return Expression.Lambda<Func<T, bool>>(aggregate(left, right), parameter);
    }
}