using System.Linq.Expressions;
using static Webinex.Asky.FilterOperator;

namespace Webinex.Asky;

public static class AskyExpressionFactory
{
    public static Expression<Func<T, bool>> Create<T>(
        IAskyFieldMap<T> fieldMap,
        FilterRule filter)
    {
        fieldMap = fieldMap ?? throw new ArgumentNullException(nameof(fieldMap));
        filter = filter ?? throw new ArgumentNullException(nameof(filter));

        return filter switch
        {
            BoolFilterRule boolFilterRule => Create(fieldMap, boolFilterRule),
            ValueFilterRule filterRule => Create(fieldMap, filterRule),
            CollectionFilterRule collectionFilterRule => Create(fieldMap, collectionFilterRule),
            ChildCollectionFilterRule childCollectionFilterRule => Create(fieldMap, childCollectionFilterRule),
            _ => throw new ArgumentOutOfRangeException(nameof(filter)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(IAskyFieldMap<T> fieldMap, BoolFilterRule filter)
    {
        var expressions = filter.Children.Select(x => Create(fieldMap, x)).ToArray();

        return filter.Operator switch
        {
            OR => BoolExpressions.Or(expressions),
            AND => BoolExpressions.And(expressions),
            _ => throw new ArgumentOutOfRangeException(nameof(filter)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(IAskyFieldMap<T> fieldMap, ValueFilterRule filter)
    {
        var field = fieldMap.Resolve(filter.FieldId);
        var value = filter.Value;

        return filter.Operator switch
        {
            EQ => FilterExpressions.Eq(field, value),
            NOT_EQ => FilterExpressions.NotEq(field, value),
            GTE => FilterExpressions.Gte(field, value),
            GT => FilterExpressions.Gt(field, value),
            LTE => FilterExpressions.Lte(field, value),
            LT => FilterExpressions.Lt(field, value),
            CONTAINS => FilterExpressions.Contains(field, value),
            NOT_CONTAINS => FilterExpressions.NotContains(field, value),
            _ => throw new ArgumentOutOfRangeException(nameof(filter.Operator)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(IAskyFieldMap<T> fieldMap, CollectionFilterRule filter)
    {
        var field = fieldMap.Resolve(filter.FieldId);
        var values = filter.Values;

        return filter.Operator switch
        {
            IN => FilterExpressions.In(field, values),
            NOT_IN => FilterExpressions.NotIn(field, values),
            _ => throw new ArgumentOutOfRangeException(nameof(filter.Operator)),
        };
    }

    private static Expression<Func<T, bool>> Create<T>(IAskyFieldMap<T> fieldMap, ChildCollectionFilterRule rule)
    {
        var field = fieldMap.Resolve(rule.FieldId);
        var collectionValueType = LambdaExpressions.ReturnCollectionValueType(field);
        var factoryType =
            typeof(AskyChildCollectionExpressionFactory<,>).MakeGenericType(typeof(T), collectionValueType);
        var factory = (IAskyChildCollectionExpressionFactory<T>)Activator.CreateInstance(factoryType, fieldMap);

        return rule.Operator switch
        {
            ANY => factory.Any(field, rule.Rule),
            ALL => factory.All(field, rule.Rule),
            _ => throw new ArgumentOutOfRangeException(nameof(rule.Operator)),
        };
    }
}