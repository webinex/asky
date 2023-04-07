using System.Linq.Expressions;
using System.Reflection;

namespace Webinex.Asky;

internal static class LambdaExpressions
{
    private static readonly MethodInfo REPLACE_RETURN_TYPE_TO_TYPED_INTERNAL_METHOD_INFO =
        typeof(LambdaExpressions).GetMethod(nameof(ReplaceReturnTypeToTypedInternal),
            BindingFlags.Static | BindingFlags.NonPublic)!;

    internal static Type ReturnType<TEntity>(Expression<Func<TEntity, object>> selector)
    {
        selector = selector ?? throw new ArgumentNullException(nameof(selector));

        if (selector.Body is MethodCallExpression methodCall)
        {
            return methodCall.Method.ReturnType;
        }
            
        return ((PropertyInfo)PropertyAccessExpression(selector.Body).Member).PropertyType;
    }

    internal static Type ReturnCollectionValueType<TEntity>(Expression<Func<TEntity, object>> selector)
    {
        selector = selector ?? throw new ArgumentNullException(nameof(selector));
        var collectionType = ReturnType(selector);
        
        var valueType = collectionType
            .GetInterfaces()
            .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(t => t.GetGenericArguments()[0])
            .FirstOrDefault();

        return valueType ??
               throw new InvalidOperationException($"{collectionType.Name} doesn't inherit generic enumerable");
    }

    internal static object ReplaceReturnTypeToTyped<TEntity>(Expression<Func<TEntity, object>> selector)
    {
        selector = selector ?? throw new ArgumentNullException(nameof(selector));
        var keyType = ReturnType(selector);
        return REPLACE_RETURN_TYPE_TO_TYPED_INTERNAL_METHOD_INFO.MakeGenericMethod(typeof(TEntity), keyType)
            .Invoke(null, new object[] { selector })!;
    }

    private static Expression<Func<TEntity, TKey>> ReplaceReturnTypeToTypedInternal<TEntity, TKey>(
        Expression<Func<TEntity, object>> selector)
    {
        var normalizedSelector = PropertyAccessExpression(selector.Body);
        return Expression.Lambda<Func<TEntity, TKey>>(normalizedSelector, selector.Parameters.ToArray());
    }

    internal static Expression PropertyAccessExpression<TEntity>(
        Expression<Func<TEntity, object>> selector,
        ParameterExpression parameter)
    {
        return BoolExpressions.ReplaceParameter(
            PropertyAccessExpression(selector.Body),
            selector.Parameters[0],
            parameter);
    }

    internal static Expression<Func<TEntity, object>> Child<TEntity, TChild>(
        Expression<Func<TEntity, TChild>> selector,
        Expression<Func<TChild, object>> childSelector)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var propertyAccessExpression = PropertyAccessExpression(selector, parameter);
        var newSelector =
            new ReplaceExpressionVisitor(childSelector.Parameters[0], propertyAccessExpression).Visit(childSelector.Body)!;
        return Expression.Lambda<Func<TEntity, object>>(newSelector, parameter);
    }

    private static Expression PropertyAccessExpression<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector,
        ParameterExpression parameter)
    {
        return ReplaceParameter(
            PropertyAccessExpression(selector.Body),
            selector.Parameters[0],
            parameter);
    }

    private static Expression ReplaceParameter(
        Expression expression,
        ParameterExpression oldParameter,
        ParameterExpression newParameter)
    {
        return new ReplaceExpressionVisitor(oldParameter, newParameter).Visit(expression)!;
    }

    internal static MemberExpression PropertyAccessExpression(Expression expression)
    {
        switch (expression)
        {
            case MemberExpression memberExpression:
                if (!memberExpression.Member.MemberType.HasFlag(MemberTypes.Property))
                    throw new InvalidOperationException($"Member {memberExpression.Member.Name} isn't a property");
                return memberExpression;

            case UnaryExpression unaryExpression:
                if (unaryExpression.NodeType == ExpressionType.Convert &&
                    unaryExpression.Operand.NodeType == ExpressionType.MemberAccess)
                    return PropertyAccessExpression(unaryExpression.Operand);
                throw new InvalidOperationException(
                    $"Unable to resolve property from unary expression {unaryExpression.NodeType}");

            default:
                throw new InvalidOperationException(
                    $"Unable to resolve property access from expression of type {expression.GetType().Name}");
        }
    }

    private class ReplaceExpressionVisitor
        : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            if (node == _oldValue)
            {
                return _newValue;
            }

            return base.Visit(node);
        }
    }
}