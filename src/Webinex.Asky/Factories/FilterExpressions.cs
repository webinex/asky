using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Webinex.Asky;

internal static class FilterExpressions
{
    private static readonly ConcurrentDictionary<Type, (Func<object, object> accessor, MemberInfo memberInfo)>
        ValueContainerFactoriesCache = new();

    public static Expression<Func<TEntity, bool>> Eq<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var valueType = LambdaExpressions.ReturnType(selector);
        var propertyExpression = LambdaExpressions.Accessor(selector, parameter);

        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.Equal(
                propertyExpression,
                WrapValueToContainerMember(value, valueType)),
            parameter);
    }

    public static Expression<Func<TEntity, bool>> NotEq<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var eq = Eq(selector, value);
        return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(eq.Body), eq.Parameters);
    }

    public static Expression<Func<TEntity, bool>> In<TEntity>(
        Expression<Func<TEntity, object>> selector,
        IEnumerable<object> values)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);
        var valueType = LambdaExpressions.ReturnType(selector);

        var containsMethodInfo =
            typeof(Enumerable)
                .GetMethods()
                .Where(x => x.Name == nameof(Enumerable.Contains) && x.IsPublic && x.IsStatic)
                .Single(x => x.GetParameters().Length == 2)
                .MakeGenericMethod(valueType);

        var castMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!.MakeGenericMethod(valueType);
        var typedValues = castMethodInfo.Invoke(null, new object[] { values.ToArray() });

        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.Call(
                containsMethodInfo,
                WrapValueToContainerMember(typedValues, typeof(IEnumerable<>).MakeGenericType(valueType)),
                propertyAccessExpression
            ),
            parameter);
    }

    public static Expression<Func<TEntity, bool>> NotIn<TEntity>(
        Expression<Func<TEntity, object>> selector,
        IEnumerable<object> values)
    {
        var inExpression = In(selector, values);
        return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(inExpression.Body), inExpression.Parameters);
    }

    public static Expression<Func<TEntity, bool>> Lte<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var valueType = LambdaExpressions.ReturnType(selector);
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);

        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.LessThanOrEqual(
                propertyAccessExpression,
                WrapValueToContainerMember(value, valueType)),
            parameter);
    }

    public static Expression<Func<TEntity, bool>> Lt<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var valueType = LambdaExpressions.ReturnType(selector);
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);

        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.LessThan(
                propertyAccessExpression,
                WrapValueToContainerMember(value, valueType)),
            parameter);
    }

    public static Expression<Func<TEntity, bool>> Gte<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var valueType = LambdaExpressions.ReturnType(selector);
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);

        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.GreaterThanOrEqual(
                propertyAccessExpression,
                WrapValueToContainerMember(value, valueType)),
            parameter);
    }

    public static Expression<Func<TEntity, bool>> Gt<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var valueType = LambdaExpressions.ReturnType(selector);
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);

        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.GreaterThan(
                propertyAccessExpression,
                WrapValueToContainerMember(value, valueType)),
            parameter);
    }

    public static Expression<Func<TEntity, bool>> Contains<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var valueType = LambdaExpressions.ReturnType(selector);
        if (valueType != typeof(string))
        {
            throw new InvalidOperationException(
                $"Contains operator available only for string type. Received: {valueType.FullName}");
        }

        var parameter = Expression.Parameter(typeof(TEntity));
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);

        var method = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        if (method == null)
        {
            throw new InvalidOperationException();
        }

        var valueConstantExpression = WrapValueToContainerMember(value, typeof(string));
        var containsResultExpression = Expression.Call(propertyAccessExpression, method, valueConstantExpression);
        return Expression.Lambda<Func<TEntity, bool>>(containsResultExpression, parameter);
    }

    public static Expression<Func<TEntity, bool>> NotContains<TEntity>(
        Expression<Func<TEntity, object>> selector,
        object value)
    {
        var valueType = LambdaExpressions.ReturnType(selector);
        if (valueType != typeof(string))
        {
            throw new InvalidOperationException(
                $"Not contains operator available only for string type. Received: {valueType.FullName}");
        }

        var containsExpression = Contains(selector, value);
        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.Not(containsExpression.Body),
            containsExpression.Parameters);
    }

    public static Expression<Func<TEntity, bool>> Any<TEntity>(
        Expression<Func<TEntity, object>> selector,
        LambdaExpression predicate)
    {
        var collectionValueType = LambdaExpressions.ReturnCollectionValueType(selector);

        var anyMethod = typeof(Enumerable).GetMethods()
            .Where(x => x.Name == nameof(Enumerable.Any)).Single(x => x.GetParameters().Length == 2)
            .MakeGenericMethod(collectionValueType);

        var parameter = Expression.Parameter(typeof(TEntity));
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);

        var methodCallExpression = Expression.Call(null, anyMethod, new[] { propertyAccessExpression, predicate });
        return Expression.Lambda<Func<TEntity, bool>>(methodCallExpression, parameter);
    }

    public static Expression<Func<TEntity, bool>> All<TEntity>(
        Expression<Func<TEntity, object>> selector,
        LambdaExpression predicate)
    {
        var collectionValueType = LambdaExpressions.ReturnCollectionValueType(selector);

        var anyMethod = typeof(Enumerable).GetMethods()
            .Where(x => x.Name == nameof(Enumerable.All)).Single(x => x.GetParameters().Length == 2)
            .MakeGenericMethod(collectionValueType);

        var parameter = Expression.Parameter(typeof(TEntity));
        var propertyAccessExpression = LambdaExpressions.Accessor(selector, parameter);

        var methodCallExpression = Expression.Call(null, anyMethod, new[] { propertyAccessExpression, predicate });
        return Expression.Lambda<Func<TEntity, bool>>(methodCallExpression, parameter);
    }

    /// <summary>
    /// It make sense to wrap value to container instead of using Expression.Constant directly, because in this case,
    /// tools like Entity Framework later will be able to optimize it.
    /// </summary>
    private static MemberExpression WrapValueToContainerMember(object value, Type type)
    {
        var (accessor, memberInfo) = ValueContainerFactoriesCache.GetOrAdd(type, (Type t) =>
        {
            var param = Expression.Parameter(typeof(object));
            var containerType = typeof(Tuple<>).MakeGenericType(t);
            var ctorInfo = containerType.GetConstructor(new[] { t }) ?? throw new InvalidOperationException();

            var accessor = Expression.Lambda<Func<object, object>>(
                Expression.New(ctorInfo, Expression.Convert(param, t)),
                param);
            var memberInfo = containerType.GetMember(nameof(Tuple<object>.Item1)).First();
            
            return (accessor.Compile(), memberInfo);
        });

        var containerInstance = accessor(value);
        return Expression.MakeMemberAccess(Expression.Constant(containerInstance), memberInfo);
    }
}