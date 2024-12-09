using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Webinex.Asky;

#pragma warning disable CS0660, CS0661
public class SortRule : EqualityComparable
#pragma warning restore CS0660, CS0661
{
    private static readonly JsonSerializerOptions DEFAULT_JSON_SERIALIZER_OPTIONS = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };
        
    public SortRule(string fieldId, SortDir dir)
    {
        FieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        Dir = dir;

        if (!Enum.IsDefined(typeof(SortDir), dir))
            throw new ArgumentException($"{dir} is not defined", nameof(dir));
    }

    public string FieldId { get; }
        
    public SortDir Dir { get; }

    public static SortRule Asc(string fieldId)
    {
        fieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        return new SortRule(fieldId, SortDir.Asc);
    }

    public static SortRule Desc(string fieldId)
    {
        fieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        return new SortRule(fieldId, SortDir.Desc);
    }

    public static SortRule? FromJson(string? jsonString, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return null;

        options ??= DEFAULT_JSON_SERIALIZER_OPTIONS;

        return JsonSerializer.Deserialize<SortRule>(jsonString!, options);
    }

    public static SortRule[]? FromJsonArray(string? jsonString, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return null;

        options ??= DEFAULT_JSON_SERIALIZER_OPTIONS;

        return JsonSerializer.Deserialize<SortRule[]>(jsonString!, options);
    }

    public static IReadOnlyCollection<SortRule>? FromJson(JsonNode? jNode, JsonSerializerOptions? options = null)
    {
        if (jNode == null)
            return null;
        
        options ??= DEFAULT_JSON_SERIALIZER_OPTIONS;

        if (jNode is JsonArray jArray)
            return jArray.Deserialize<IReadOnlyCollection<SortRule>>(options);

        if (jNode is JsonObject jObject)
            return new[] { jObject.Deserialize<SortRule>(options) };
        
        throw new ArgumentException("Expected array or object", nameof(jNode));
    }

    public static IReadOnlyCollection<SortRule>? FromJson(JsonElement? jElement, JsonSerializerOptions? options)
    {
        if (jElement == null)
            return null;
        
        var jNode = JsonNode.Parse(jElement.Value.GetRawText())!;
        return FromJson(jNode, options);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Dir;
        yield return FieldId;
    }

    public static bool operator ==(SortRule? left, SortRule? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(SortRule? left, SortRule? right)
    {
        return NotEqualOperator(left, right);
    }
}