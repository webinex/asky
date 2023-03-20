using System.Text.Json;
using System.Text.Json.Serialization;

namespace Webinex.Asky;

public class AskyFilterJsonConverter<T> : JsonConverter<FilterRule>
{
    private readonly IAskyFieldMap<T> _fieldMap;

    public AskyFilterJsonConverter(IAskyFieldMap<T> fieldMap)
    {
        _fieldMap = fieldMap;
    }

    public override FilterRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var readerAtStart = reader;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"`{nameof(FilterRule)}` might be object.");

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        return DeserializeFilterRule(readerAtStart, jsonDocument, options);
    }

    private FilterRule DeserializeFilterRule(
        Utf8JsonReader readerAtStart,
        JsonDocument jDocument,
        JsonSerializerOptions options)
    {
        var filterBase = jDocument.Deserialize<FilterBase>(options);
        var @operator = filterBase!.Operator;

        if (FilterOperator.ALL_VALUES.Contains(@operator))
            return DeserializeFilterRuleWithValue(typeof(ValueFilterRule<>), readerAtStart, options, filterBase);

        if (FilterOperator.ALL_BOOL.Contains(@operator))
            return JsonSerializer.Deserialize<BoolFilterRule>(ref readerAtStart, options)!;

        if (FilterOperator.ALL_CHILD_COLLECTION.Contains(@operator))
            return JsonSerializer.Deserialize<ChildCollectionFilterRule>(ref readerAtStart, options)!;

        if (FilterOperator.ALL_COLLECTION.Contains(@operator))
            return DeserializeFilterRuleWithValue(typeof(CollectionFilterRule<>), readerAtStart, options, filterBase);

        throw new InvalidOperationException($"Unknown operator {@operator}");
    }

    private FilterRule DeserializeFilterRuleWithValue(
        Type valuedRuleType,
        Utf8JsonReader readerAtStart,
        JsonSerializerOptions options,
        FilterBase filterBase)
    {
        var valueType = ResolveValueType(filterBase);

        var valueFilterRuleType = valuedRuleType.MakeGenericType(typeof(T), valueType);
        var valueFilterRule =
            (IValueFilterRuleConvertible)JsonSerializer.Deserialize(ref readerAtStart, valueFilterRuleType, options)!;

        return valueFilterRule.ToFilterRule();
    }

    private Type ResolveValueType(FilterBase filterBase)
    {
        if (filterBase.FieldId == null)
        {
            throw new InvalidOperationException(
                $"{nameof(ValueFilterRule)} might have {nameof(ValueFilterRule.FieldId)}");
        }
        
        var expression = _fieldMap[filterBase.FieldId!];
        if (expression == null)
        {
            throw new InvalidOperationException($"No field map specified for field {filterBase.FieldId}");
        }

        return LambdaExpressions.ReturnType(expression);
    }

    public override void Write(Utf8JsonWriter writer, FilterRule value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }

    private class FilterBase
    {
        public string Operator { get; set; } = null!;
        public string? FieldId { get; set; }
    }
    
    private interface IValueFilterRuleConvertible
    {
        FilterRule ToFilterRule();
    }

    private class ValueFilterRule<TValue> : IValueFilterRuleConvertible
    {
        public string FieldId { get; set; } = null!;
        public string Operator { get; set; } = null!;
        public TValue Value { get; set; } = default!;

        public FilterRule ToFilterRule() => new ValueFilterRule(FieldId, Operator, Value!);
    }
    
    private class CollectionFilterRule<TValue> : IValueFilterRuleConvertible
    {
        public string Operator { get; set; } = null!;
        public string FieldId { get; set; } = null!;
        public IEnumerable<TValue> Values { get; set; } = null!;
        public FilterRule ToFilterRule() => new CollectionFilterRule(FieldId, Operator, Values.Cast<object>());
    }
}