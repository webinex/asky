// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Webinex.Asky;

public class ValueFilterRule : FilterRule
{
    protected ValueFilterRule()
    {
    }

    public ValueFilterRule(string fieldId, string @operator, object value)
    {
        FieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        Value = value;

        if (!FilterOperator.ALL_VALUES.Contains(Operator))
            throw new ArgumentException($"Invalid value filter operator {@operator}", nameof(@operator));
    }

    public string FieldId { get; protected set; } = null!;

    public string Operator { get; protected set; } = null!;

    public object Value { get; protected set; } = null!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FieldId;
        yield return Operator;
        yield return Value;
    }
}