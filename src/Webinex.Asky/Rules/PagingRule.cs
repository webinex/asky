using System.Text.Json;
using System.Text.Json.Nodes;

namespace Webinex.Asky;

#pragma warning disable CS0660, CS0661
public class PagingRule : EqualityComparable
#pragma warning restore CS0660, CS0661
{
    private static readonly JsonSerializerOptions DEFAULT_JSON_SERIALIZER_OPTIONS = new()
    {
        PropertyNameCaseInsensitive = true,
    };


    public PagingRule(int skip, int take)
    {
        Skip = skip;
        Take = take;

        if (Skip < 0)
            throw new ArgumentOutOfRangeException(nameof(skip), "Might not be less then 0");

        if (Take < 0)
            throw new ArgumentOutOfRangeException(nameof(skip), "Might not be less then 0");
    }

    public int Skip { get; }

    public int Take { get; }

    public static PagingRule TakeFirst(int take)
    {
        return new PagingRule(0, take);
    }

    public static PagingRule? FromJson(string? jsonString, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return null;

        options ??= DEFAULT_JSON_SERIALIZER_OPTIONS;
        return JsonSerializer.Deserialize<PagingRule>(jsonString!, options);
    }

    public static PagingRule? FromJson(JsonNode? jNode, JsonSerializerOptions? options = null)
    {
        if (jNode == null)
            return null;

        options ??= DEFAULT_JSON_SERIALIZER_OPTIONS;
        return jNode.Deserialize<PagingRule>(options);
    }

    public static PagingRule? FromJson(JsonElement? jElement, JsonSerializerOptions? options = null)
    {
        if (jElement == null)
            return null;

        var jNode = JsonNode.Parse(jElement.Value.GetRawText());
        return FromJson(jNode, options);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Skip;
        yield return Take;
    }

    public static bool operator ==(PagingRule? left, PagingRule? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(PagingRule? left, PagingRule? right)
    {
        return NotEqualOperator(left, right);
    }
}