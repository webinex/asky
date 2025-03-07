using System;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class BoolValueFilterTests : ValueFilterTestsBase<bool>
{
    [Test]
    public void WhenEq_True()
    {
        WithValues(true, false, true);
        
        Run(FilterRule.Eq(FieldId, true));

        Result.Should().BeEquivalentTo(new[] { true, true });
    }

    [Test]
    public void WhenEq_False()
    {
        WithValues(true, false, true);
        
        Run(FilterRule.Eq(FieldId, false));

        Result.Should().BeEquivalentTo(new[] { false });
    }
    
    [Test]
    public void WhenEq_NoMatch()
    {
        WithValues(false, false, false);
        
        Run(FilterRule.Eq(FieldId, true));

        Result.Should().BeEmpty();
    }

    [Test]
    public void WhenNotEq()
    {
        WithValues(false, true, false, true);
        
        Run(FilterRule.NotEq(FieldId, false));

        Result.Should().BeEquivalentTo(new[] { true, true });
    }

    [Test]
    public void WhenIn()
    {
        WithValues(true, true, false);
        
        Run(FilterRule.In(FieldId, new[] { true }));

        Result.Should().BeEquivalentTo(new[] { true, true });
    }

    [Test]
    public void WhenInAll()
    {
        WithValues(true, true, false);
        
        Run(FilterRule.In(FieldId, new[] { true, false }));

        Result.Should().BeEquivalentTo(new[] { true, true, false });
    }

    [Test]
    public void WhenNotIn()
    {
        WithValues(true, true, false);
        
        Run(FilterRule.NotIn(FieldId, new[] { true }));

        Result.Should().BeEquivalentTo(new[] { false });
    }

    [TestCase(FilterOperator.GT)]
    [TestCase(FilterOperator.GTE)]
    [TestCase(FilterOperator.LT)]
    [TestCase(FilterOperator.LTE)]
    [TestCase(FilterOperator.CONTAINS)]
    [TestCase(FilterOperator.NOT_CONTAINS)]
    [TestCase(FilterOperator.STARTS_WITH)]
    [TestCase(FilterOperator.NOT_STARTS_WITH)]
    public void WhenInvalidOperator_ShouldThrow(string @operator)
    {
        WithValues(true, false);

        Assert.Throws<InvalidOperationException>(() => Run(new ValueFilterRule(FieldId, @operator, true)));
    }
}