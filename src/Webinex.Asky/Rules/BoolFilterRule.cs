namespace Webinex.Asky;

#pragma warning disable CS0660, CS0661
public class BoolFilterRule : FilterRule
#pragma warning restore CS0660, CS0661
{
    protected BoolFilterRule()
    {
    }
        
    public BoolFilterRule(string @operator, FilterRule[] children)
    {
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        Children = children ?? throw new ArgumentNullException(nameof(children));

        if (!FilterOperator.ALL_BOOL.Contains(@operator))
            throw new ArgumentException($"Unknown {nameof(BoolFilterRule)} operator {@operator}", nameof(@operator));

        if (children.Length < 2)
            throw new ArgumentException("Might have at least 2 children", nameof(children));
    }

    public string Operator { get; protected set; } = null!;
        
    public FilterRule[] Children { get; protected set; } = null!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return (Children ?? Array.Empty<object>()).Concat(new[] { Operator });
    }

    public static bool operator ==(BoolFilterRule? left, BoolFilterRule? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(BoolFilterRule? left, BoolFilterRule? right)
    {
        return NotEqualOperator(left, right);
    }
}