using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal abstract class DecimalValueFilterTestsBase<T> : ValueFilterTestsBase<T>
{
    [Test]
    public void WhenEq()
    {
        WithValues(Value(1.3), Value(2.4), Value(-0.3), Value(0));

        Run(FilterRule.Eq(FieldId, Value(-0.3)));

        Result.Should().BeEquivalentTo(new[] { Value(-0.3) });
    }

    [Test]
    public void WhenNotEq()
    {
        WithValues(Value(1.3), Value(2.4), Value(-0.3), Value(0));

        Run(FilterRule.NotEq(FieldId, Value(1.3)));

        Result.Should().BeEquivalentTo(new[] { Value(2.4), Value(-0.3), Value(0) });
    }

    [Test]
    public void WhenGt()
    {
        WithValues(Value(1.3), Value(2.4), Value(-0.3), Value(0));

        Run(FilterRule.Gt(FieldId, Value(0)));

        Result.Should().BeEquivalentTo(new[] { Value(1.3), Value(2.4) });
    }

    [Test]
    public void WhenGte()
    {
        WithValues(Value(1.3), Value(2.4), Value(-0.3), Value(0));

        Run(FilterRule.Gte(FieldId, Value(0)));

        Result.Should().BeEquivalentTo(new[] { Value(1.3), Value(2.4), Value(0) });
    }

    [Test]
    public void WhenLt()
    {
        WithValues(Value(1.3), Value(2.4), Value(-0.3), Value(0));

        Run(FilterRule.Lt(FieldId, Value(0)));

        Result.Should().BeEquivalentTo(new[] { Value(-0.3) });
    }

    [Test]
    public void WhenLte()
    {
        WithValues(Value(1.3), Value(2.4), Value(-0.3), Value(0));

        Run(FilterRule.Lte(FieldId, Value(0)));

        Result.Should().BeEquivalentTo(new[] { Value(-0.3), Value(0) });
    }

    [Test]
    public void WhenIn()
    {
        WithValues(Value(1), Value(-1), Value(0), Value(3));
        
        Run(FilterRule.In(FieldId, new[] { Value(1), Value(0), Value(6) }));

        Result.Should().BeEquivalentTo(new[] { Value(1), Value(0) });
    }

    [Test]
    public void WhenNotIn()
    {
        WithValues(Value(1), Value(-1), Value(0), Value(3));
        
        Run(FilterRule.NotIn(FieldId, new[] { Value(1), Value(0), Value(6) }));

        Result.Should().BeEquivalentTo(new[] { Value(-1), Value(3) });
    }

    [TestCase(FilterOperator.CONTAINS)]
    [TestCase(FilterOperator.NOT_CONTAINS)]
    [TestCase(FilterOperator.STARTS_WITH)]
    [TestCase(FilterOperator.NOT_STARTS_WITH)]
    public void WhenInvalidOperator_ShouldThrow(string @operator)
    {
        WithValues(Value(1.3), Value(2.4));

        Assert.Throws<InvalidOperationException>(() => Run(new ValueFilterRule(FieldId, @operator, 0)));
    }

    protected T Value(double value) =>
        (T)Convert.ChangeType(value.ToString(CultureInfo.InvariantCulture), typeof(T), CultureInfo.InvariantCulture);
}

internal class DecimalValueFilterTests : DecimalValueFilterTestsBase<decimal>
{
}

internal class DoubleValueFilterTests : DecimalValueFilterTestsBase<double>
{
}

internal class FloatValueFilterTests : DecimalValueFilterTestsBase<float>
{
}