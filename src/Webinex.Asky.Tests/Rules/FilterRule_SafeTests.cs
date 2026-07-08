using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Rules;

public class FilterRule_SafeTests
{
    [Test]
    public void AndSafe_WhenAllRulesNull_ShouldReturnNull()
    {
        FilterRule.AndSafe(null, null, null).Should().BeNull();
    }

    [Test]
    public void AndSafe_WhenSingleRuleProvided_ShouldReturnSameRule()
    {
        var rule = FilterRule.Eq("name", "John");

        FilterRule.AndSafe(null, rule, null).Should().BeSameAs(rule);
    }

    [Test]
    public void AndSafe_WhenSeveralRulesProvided_ShouldCombineNonNullRules()
    {
        var r1 = FilterRule.Eq("name", "John");
        var r2 = FilterRule.NotEq("name", "Jane");

        FilterRule.AndSafe(r1, null, r2)
            .Should()
            .BeEquivalentTo(FilterRule.And(r1, r2));
    }

    [Test]
    public void OrSafe_WhenAllRulesNull_ShouldReturnNull()
    {
        FilterRule.OrSafe(null, null, null).Should().BeNull();
    }

    [Test]
    public void OrSafe_WhenSingleRuleProvided_ShouldReturnSameRule()
    {
        var rule = FilterRule.Eq("name", "John");

        FilterRule.OrSafe(null, rule, null).Should().BeSameAs(rule);
    }

    [Test]
    public void OrSafe_WhenSeveralRulesProvided_ShouldCombineNonNullRules()
    {
        var r1 = FilterRule.Eq("name", "John");
        var r2 = FilterRule.NotEq("name", "Jane");

        FilterRule.OrSafe(r1, null, r2)
            .Should()
            .BeEquivalentTo(FilterRule.Or(r1, r2));
    }
}
