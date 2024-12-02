using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Webinex.Asky;

internal static class FilterExpressions
{
    private static readonly ConcurrentDictionary<Type, Func<object, object>> ValueContainerFactoriesCache = new()
    {
        [typeof(string)] = v => new ValueContainer<string>((string)v),
        [typeof(bool)] = v => new ValueContainer<bool>((bool)v),
        [typeof(char)] = v => new ValueContainer<char>((char)v),
        [typeof(string)] = v => new ValueContainer<string>((string)v),
        [typeof(short)] = v => new ValueContainer<short>((short)v),
        [typeof(int)] = v => new ValueContainer<int>((int)v),
        [typeof(long)] = v => new ValueContainer<long>((long)v),
        [typeof(double)] = v => new ValueContainer<double>((double)v),
        [typeof(float)] = v => new ValueContainer<float>((float)v),
        [typeof(decimal)] = v => new ValueContainer<decimal>((decimal)v),
        [typeof(Guid)] = v => new ValueContainer<Guid>((Guid)v),
        [typeof(DateTimeOffset)] = v => new ValueContainer<DateTimeOffset>((DateTimeOffset)v),
        [typeof(DateTime)] = v => new ValueContainer<DateTime>((DateTime)v),
    };

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
        var accessor = ValueContainerFactoriesCache.GetOrAdd(type, (Type t) =>
        {
            var param = Expression.Parameter(typeof(object));
            var ctorInfo = typeof(ValueContainer<>).MakeGenericType(t)
                .GetConstructor(new[] { t }) ?? throw new ArgumentNullException();

            var expression = Expression.Lambda<Func<object, object>>(
                Expression.New(ctorInfo, new[] { Expression.Convert(param, t) }),
                new[] { param });
            return expression.Compile();
        });

        var containerInstance = accessor(value);
        return Expression.MakeMemberAccess(
            Expression.Constant(containerInstance),
            containerInstance.GetType().GetMember(nameof(ValueContainer<object>.Value)).First());
    }

    /// <summary>
    /// It make no sense to use struct here, because it will be anyway allocated in the heap
    /// </summary>
    private sealed class ValueContainer<TValue>
    {
        public readonly TValue Value;

        public ValueContainer(TValue value)
        {
            Value = value;
        }
    }
}