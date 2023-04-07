namespace Webinex.Asky;

public class ChildCollectionFilterRule : FilterRule
{
    public ChildCollectionFilterRule(string @operator, string fieldId, FilterRule rule)
    {
        Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
        FieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        Rule = rule ?? throw new ArgumentNullException(nameof(rule));

        if (!FilterOperator.ALL_CHILD_COLLECTION.Contains(Operator))
            throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Not a collection operator");
    }
    
    public string Operator { get; protected set; } = null!;
    public string FieldId { get; protected set; } = null!;
    public FilterRule Rule { get; protected set; } = null!;
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Operator;
        yield return FieldId;
        yield return Rule;
    }
}