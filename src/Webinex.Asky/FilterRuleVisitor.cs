namespace Webinex.Asky;

public class FilterRuleVisitor
{
    public virtual FilterRule? Visit(ValueFilterRule valueFilterRule)
    {
        return null;
    }

    public virtual FilterRule? Visit(CollectionFilterRule collectionFilterRule)
    {
        return null;
    }

    public virtual FilterRule? Visit(ChildCollectionFilterRule childCollectionFilterRule)
    {
        return null;
    }

    public virtual FilterRule? Visit(BoolFilterRule boolFilterRule)
    {
        return null;
    }

    internal static class Executor
    {
        public static FilterRule Execute(FilterRule rule, FilterRuleVisitor visitor)
        {
            rule = rule ?? throw new ArgumentNullException(nameof(rule));
            visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));

            if (rule is ValueFilterRule valueFilterRule)
                return Visit(valueFilterRule, visitor);

            if (rule is CollectionFilterRule collectionFilterRule)
                return Visit(collectionFilterRule, visitor);

            if (rule is ChildCollectionFilterRule childCollectionFilterRule)
                return Visit(childCollectionFilterRule, visitor);

            if (rule is BoolFilterRule boolFilterRule)
                return Visit(boolFilterRule, visitor);

            return rule;
        }

        private static FilterRule Visit(ValueFilterRule valueFilterRule, FilterRuleVisitor visitor)
        {
            var result = visitor.Visit(valueFilterRule);
            return result != null ? result : valueFilterRule;
        }

        private static FilterRule Visit(CollectionFilterRule collectionFilterRule, FilterRuleVisitor visitor)
        {
            var result = visitor.Visit(collectionFilterRule);
            return result != null ? result : collectionFilterRule;
        }

        private static FilterRule Visit(ChildCollectionFilterRule childCollectionFilterRule, FilterRuleVisitor visitor)
        {
            var result = visitor.Visit(childCollectionFilterRule);
            if (result != null)
                return result;

            var newRule = Execute(childCollectionFilterRule.Rule, visitor);
            return newRule == childCollectionFilterRule.Rule
                ? childCollectionFilterRule
                : new ChildCollectionFilterRule(childCollectionFilterRule.Operator, childCollectionFilterRule.FieldId, newRule);
        }

        private static FilterRule Visit(BoolFilterRule boolFilterRule, FilterRuleVisitor visitor)
        {
            var result = visitor.Visit(boolFilterRule);
            if (result != null)
                return result;

            var newChildren = boolFilterRule.Children.Select(child => Execute(child, visitor)).ToArray();
            return boolFilterRule.Children.SequenceEqual(newChildren)
                ? boolFilterRule
                : new BoolFilterRule(boolFilterRule.Operator, newChildren.ToArray());
        }
    }
}