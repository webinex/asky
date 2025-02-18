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

        var accessExpression = Accessor(selector.Body);
        return GetAccessExpressionReturnType(accessExpression);
    }

    private static Type GetAccessExpressionReturnType(Expression expression)
    {
        switch (expression)
        {
            case MemberExpression member when member.Member.MemberType == MemberTypes.Property:
                return ((PropertyInfo)member.Member).PropertyType;
            case UnaryExpression unary when unary.NodeType == ExpressionType.Convert:
                return GetAccessExpressionReturnType(unary.Operand);
            case ConstantExpression constant:
                return constant.Type;
            case ConditionalExpression condition:
                var falseType = GetAccessExpressionReturnType(condition.IfFalse);
                var trueType = GetAccessExpressionReturnType(condition.IfTrue);
                var falseUnderlyingType = Nullable.GetUnderlyingType(falseType);
                var trueUnderlyingType = Nullable.GetUnderlyingType(trueType);
                var falseNotNullType = falseUnderlyingType ?? falseType;
                var trueNotNullType = trueUnderlyingType ?? trueType;

                if (falseNotNullType != trueNotNullType)
                    throw new InvalidOperationException(
                        $"Conditional expression types doesn't match IfFalse = {falseType.Name}, IfTrue = {trueType.Name}");
                return trueUnderlyingType != null ? trueType : falseType;
            default:
                throw new InvalidOperationException(
                    $"Unable to resolve access expression return type of expression of type {expression.GetType().Name}");
        }
    }

    public static bool IsSelectExpression<TEntity>(Expression<Func<TEntity, object>> selector)
    {
        selector = selector ?? throw new ArgumentNullException(nameof(selector));

        if (selector.Body is not MethodCallExpression methodCallExpression)
            return false;

        return methodCallExpression.Method.IsGenericMethod &&
               typeof(Enumerable).GetMethods().Where(x => x.Name == nameof(Enumerable.Select))
                   .Contains(methodCallExpression.Method.GetGenericMethodDefinition());
    }

    internal static Type ReturnCollectionValueType<TEntity>(Expression<Func<TEntity, object>> selector)
    {
        selector = selector ?? throw new ArgumentNullException(nameof(selector));
        var collectionType = ReturnType(selector);
        var valueType = TypeUtil.GetGenericEnumerableImplValueType(collectionType);

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
        var normalizedSelector = Accessor(selector.Body);
        return Expression.Lambda<Func<TEntity, TKey>>(normalizedSelector, selector.Parameters.ToArray());
    }

    internal static Expression Accessor<TEntity>(
        Expression<Func<TEntity, object>> selector,
        ParameterExpression parameter)
    {
        return ParameterReplacer.Replace(
            Accessor(selector.Body),
            selector.Parameters[0],
            parameter);
    }

    internal static Expression<Func<TEntity, object>> Child<TEntity, TChild>(
        Expression<Func<TEntity, TChild>> selector,
        Expression<Func<TChild, object>> childSelector)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var propertyAccessExpression = Accessor(selector, parameter);
        var newSelector = ParameterReplacer.Replace(childSelector.Body, childSelector.Parameters[0],
            propertyAccessExpression);
        return Expression.Lambda<Func<TEntity, object>>(newSelector, parameter);
    }

    private static Expression Accessor<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector,
        ParameterExpression parameter)
    {
        return ParameterReplacer.Replace(
            Accessor(selector.Body),
            selector.Parameters[0],
            parameter);
    }

    internal static Expression Accessor(Expression expression)
    {
        switch (expression)
        {
            case MemberExpression memberExpression:
                if (!memberExpression.Member.MemberType.HasFlag(MemberTypes.Property))
                    throw new InvalidOperationException($"Member {memberExpression.Member.Name} isn't a property");
                return memberExpression;

            case UnaryExpression unaryExpression:
                if (unaryExpression.NodeType == ExpressionType.Convert)
                    return Accessor(unaryExpression.Operand);
                throw new InvalidOperationException(
                    $"Unable to resolve property from unary expression {unaryExpression.NodeType}");

            case ConditionalExpression conditionalExpression:
                return conditionalExpression;

            default:
                throw ThrowUnableToResolve(expression);
        }
    }

    private static Exception ThrowUnableToResolve(Expression expression)
    {
        throw new InvalidOperationException(
            $"Unable to resolve property access from expression of type {expression.GetType().Name}");
    }
}