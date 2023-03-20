using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Values;

internal class NestedPropValueFilterTests
{
    private int[] _values;
    private int[] _result;

    private Tuple<Tuple<int>>[] Source => _values.Select(i => Tuple.Create(Tuple.Create(i))).ToArray();

    [Test]
    public void WhenEq()
    {
        WithValues(1, 2, 3);
        
        Run(FilterRule.Eq("x", 2));

        _result.Should().BeEquivalentTo(new[] { 2 });
    }

    [Test]
    public void WhenNotEq()
    {
        WithValues(1, 2, 3);
        
        Run(FilterRule.NotEq("x", 2));

        _result.Should().BeEquivalentTo(new[] { 1, 3 });
    }

    [Test]
    public void WhenGt()
    {
        WithValues(2, 3, 1, 0);
        
        Run(FilterRule.Gt("x", 2));

        _result.Should().BeEquivalentTo(new[] { 3 });
    }

    [Test]
    public void WhenGte()
    {
        WithValues(2, 1, 3, 0);
        
        Run(FilterRule.Gte("x", 2));

        _result.Should().BeEquivalentTo(new[] { 2, 3 });
    }

    [Test]
    public void WhenLt()
    {
        WithValues(2, -1, 3, 0);
        
        Run(FilterRule.Lt("x", 1));

        _result.Should().BeEquivalentTo(new[] { -1, 0 });
    }

    [Test]
    public void WhenLte()
    {
        WithValues(2, -1, 3, 0);
        
        Run(FilterRule.Lte("x", 2));

        _result.Should().BeEquivalentTo(new[] { -1, 0, 2});
    }

    [Test]
    public void WhenIn()
    {
        WithValues(1, -2, 3, 4);
        
        Run(FilterRule.In("x", new[] { _values[1], 6, _values[3] }));

        _result.Should().BeEquivalentTo(new[] { _values[1], _values[3] });
    }

    [Test]
    public void WhenNotIn()
    {
        WithValues(1, 0, -8, 4);
        
        Run(FilterRule.NotIn("x", new[] { _values[1], 22, _values[3] }));

        _result.Should().BeEquivalentTo(new[] { _values[0], _values[2] });
    }

    private void WithValues(params int[] values)
    {
        _values = values;
    }

    protected void Run(FilterRule filter)
    {
        var result = Source.AsQueryable().Where(new FieldMap(), filter).ToArray();
        _result = result.Select(x => x.Item1).Select(x => x.Item1).ToArray();
    }

    [SetUp]
    public void SetUp()
    {
        _values = null;
        _result = null;
    }

    private class FieldMap : IAskyFieldMap<Tuple<Tuple<int>>>
    {
        public Expression<Func<Tuple<Tuple<int>>, object>> this[string fieldId] => x => x.Item1.Item1;
    }
}