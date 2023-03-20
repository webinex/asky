using System;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class GuidValueFilterTests : ValueFilterTestsBase<Guid>
{
    [Test]
    public void WhenEq()
    {
        var value = Guid.NewGuid();
        WithValues(Guid.NewGuid(), value, Guid.NewGuid(), value);

        Run(FilterRule.Eq(FieldId, value));

        Result.Should().BeEquivalentTo(new[] { value, value });
    }

    [Test]
    public void WhenNotEq()
    {
        var value = Guid.NewGuid();
        var values = new[] { Guid.NewGuid(), value, Guid.NewGuid(), value };
        WithValues(values);

        Run(FilterRule.NotEq(FieldId, value));

        Result.Should().BeEquivalentTo(new[] { values[0], values[2] });
    }

    [Test]
    public void WhenIn()
    {
        WithValues(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        Run(FilterRule.In(FieldId, new[] { Values[1], Guid.NewGuid(), Values[3] }));

        Result.Should().BeEquivalentTo(new[] { Values[1], Values[3] });
    }

    [Test]
    public void WhenNotIn()
    {
        WithValues(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        Run(FilterRule.NotIn(FieldId, new[] { Values[1], Guid.NewGuid(), Values[3] }));

        Result.Should().BeEquivalentTo(new[] { Values[0], Values[2] });
    }

    [TestCase(FilterOperator.GT)]
    [TestCase(FilterOperator.GTE)]
    [TestCase(FilterOperator.LT)]
    [TestCase(FilterOperator.LTE)]
    [TestCase(FilterOperator.CONTAINS)]
    [TestCase(FilterOperator.NOT_CONTAINS)]
    public void WhenInvalidOperator_ShouldThrow(string @operator)
    {
        WithValues(Guid.NewGuid(), Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => Run(new ValueFilterRule(FieldId, @operator, Guid.NewGuid())));
    }
}