using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Webinex.Asky;

#pragma warning disable CS0660, CS0661
public abstract class FilterRule : EqualityComparable
#pragma warning restore CS0660, CS0661
{
    private static readonly JsonSerializerOptions DEFAULT_JSON_SERIALIZER_OPTIONS = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public FilterRule Replace(FilterRuleVisitor visitor)
    {
        return FilterRuleVisitor.Executor.Execute(this, visitor);
    }

    public static bool operator ==(FilterRule? left, FilterRule? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(FilterRule? left, FilterRule? right)
    {
        return NotEqualOperator(left, right);
    }

    public static ValueFilterRule Eq(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.EQ, value);
    }

    public static ValueFilterRule NotEq(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.NOT_EQ, value);
    }

    public static ValueFilterRule Gt(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.GT, value);
    }

    public static ValueFilterRule Gte(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.GTE, value);
    }

    public static ValueFilterRule Lt(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.LT, value);
    }

    public static ValueFilterRule Lte(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.LTE, value);
    }

    public static ValueFilterRule Contains(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.CONTAINS, value);
    }

    public static ValueFilterRule Contains(string fieldId, string value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.CONTAINS, value);
    }

    public static ValueFilterRule NotContains(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.NOT_CONTAINS, value);
    }

    public static ValueFilterRule NotContains(string fieldId, string value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.NOT_CONTAINS, value);
    }

    public static ValueFilterRule StartsWith(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.STARTS_WITH, value);
    }

    public static ValueFilterRule StartsWith(string fieldId, string value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.STARTS_WITH, value);
    }

    public static ValueFilterRule NotStartsWith(string fieldId, object value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.NOT_STARTS_WITH, value);
    }

    public static ValueFilterRule NotStartsWith(string fieldId, string value)
    {
        return new ValueFilterRule(fieldId, FilterOperator.NOT_STARTS_WITH, value);
    }

    public static CollectionFilterRule In<T>(string fieldId, T[] values)
    {
        return new CollectionFilterRule(fieldId, FilterOperator.IN, values.Cast<object>().ToArray());
    }

    public static CollectionFilterRule In(string fieldId, object[] values)
    {
        return new CollectionFilterRule(fieldId, FilterOperator.IN, values);
    }

    public static CollectionFilterRule NotIn<T>(string fieldId, T[] values)
    {
        return new CollectionFilterRule(fieldId, FilterOperator.NOT_IN, values.Cast<object>().ToArray());
    }

    public static CollectionFilterRule NotIn(string fieldId, object[] values)
    {
        return new CollectionFilterRule(fieldId, FilterOperator.NOT_IN, values);
    }

    public static BoolFilterRule Or(IEnumerable<FilterRule> filters)
    {
        return new BoolFilterRule(FilterOperator.OR, filters.ToArray());
    }

    public static BoolFilterRule Or(FilterRule x, FilterRule y, params FilterRule[] more)
    {
        return new BoolFilterRule(FilterOperator.OR, new[] { x, y }.Concat(more).ToArray());
    }

    public static ChildCollectionFilterRule Any(string fieldId, FilterRule rule)
    {
        return new ChildCollectionFilterRule(FilterOperator.ANY, fieldId, rule);
    }

    public static ChildCollectionFilterRule All(string fieldId, FilterRule rule)
    {
        return new ChildCollectionFilterRule(FilterOperator.ALL, fieldId, rule);
    }

    public static BoolFilterRule And(FilterRule x, FilterRule y, params FilterRule[] more)
    {
        return new BoolFilterRule(FilterOperator.AND, new[] { x, y }.Concat(more).ToArray());
    }

    public static BoolFilterRule And(IEnumerable<FilterRule> filters)
    {
        return new BoolFilterRule(FilterOperator.AND, filters.ToArray());
    }

    public static FilterRule? FromJson(string? jsonString, JsonSerializerOptions options)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return null;
        }

        return JsonSerializer.Deserialize<FilterRule>(jsonString!, options);
    }

    public static FilterRule? FromJson<TEntity>(string? jsonString, IAskyFieldMap<TEntity> fieldMap)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return null;
        }

        var options = GetJsonSerializerOptions(fieldMap);
        return JsonSerializer.Deserialize<FilterRule>(jsonString!, options);
    }
    
    public static FilterRule? FromJson<TEntity>(JsonElement? jsonElement, IAskyFieldMap<TEntity> fieldMap)
    {
        return jsonElement != null ? FromJson(jsonElement.Value.GetRawText(), fieldMap) : null;
    }

    public static FilterRule? FromJson<TEntity>(JsonNode? jsonNode, IAskyFieldMap<TEntity> fieldMap)
    {
        var options = GetJsonSerializerOptions(fieldMap);
        return jsonNode?.Deserialize<FilterRule>(options);
    }
    
    private static JsonSerializerOptions GetJsonSerializerOptions<TEntity>(IAskyFieldMap<TEntity> fieldMap)
    {
        return new JsonSerializerOptions(DEFAULT_JSON_SERIALIZER_OPTIONS)
        {
            Converters = { new AskyFilterJsonConverter<TEntity>(fieldMap) },
        };
    }
}