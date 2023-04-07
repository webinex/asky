namespace Webinex.Asky;

#pragma warning disable CS0660, CS0661
public class CollectionFilterRule : FilterRule
#pragma warning restore CS0660, CS0661
{
    protected CollectionFilterRule()
    {
    }

    public CollectionFilterRule(string fieldId, string @operator, IEnumerable<object> values)
    {
        FieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        Values = values ?? throw new ArgumentNullException(nameof(values));

        if (!FilterOperator.ALL_COLLECTION.Contains(@operator))
        {
            throw new ArgumentException($"Unknown {nameof(CollectionFilterRule)} operator {@operator}",
                nameof(@operator));
        }
    }

    public string Operator { get; protected set; } = null!;

    public string FieldId { get; protected set; } = null!;

    public IEnumerable<object> Values { get; protected set; } = null!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return (Values ?? Array.Empty<object>()).Concat(new[] { Operator, FieldId });
    }

    public static bool operator ==(CollectionFilterRule? left, CollectionFilterRule? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(CollectionFilterRule? left, CollectionFilterRule? right)
    {
        return NotEqualOperator(left, right);
    }
}