using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Child;

public class AskyChildFieldMapTests
{
    private static readonly ParentFieldMap FIELD_MAP = new ParentFieldMap();
    
    private static readonly Parent[] ITEMS = {
        new()
        {
            Name = "1",
            Child1 = new Child1
            {
                Name = "1-1",
                Child2 = new Child2
                {
                    Name = "1-1-1",
                },
            },
        },
        new()
        {
            Name = "2",
            Child1 = new Child1
            {
                Name = "2-1",
                Child2 = new Child2
                {
                    Name = "2-1-1",
                },
            },
        },
    };

    private Parent[] _result = null!;

    [Test]
    public void WhenOneLevelDepth_ShouldBeOk()
    {
        Run(FilterRule.Eq("child.name", "2-1"));

        _result.Length.Should().Be(1);
        _result.Single().Child1.Name.Should().Be("2-1");
    }

    [Test]
    public void WhenTwoLevelDepth_ShouldBeOk()
    {
        Run(FilterRule.Eq("child.child1.name", "2-1-1"));

        _result.Length.Should().Be(1);
        _result.Single().Child1.Child2.Name.Should().Be("2-1-1");
    }

    [Test]
    public void WhenNotFoundInChild_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() => Run(FilterRule.Eq("child.blah", "2-1-1")));
    }

    private void Run(FilterRule filterRule)
    {
        _result = ITEMS.AsQueryable().Where(FIELD_MAP, filterRule).ToArray();
    }

    private class ParentFieldMap : IAskyFieldMap<Parent>
    {
        public Expression<Func<Parent, object>> this[string fieldId]
        {
            get
            {
                if (fieldId.StartsWith("child."))
                {
                    return AskyFieldMap.Forward<Parent, Child1>(x => x.Child1,
                        new Child1FieldMap(), fieldId.Substring("child.".Length));
                }

                return fieldId switch
                {
                    "name" => x => x.Name,
                    _ => null,
                };
            }
        }
    }

    private class Child1FieldMap : IAskyFieldMap<Child1>
    {
        public Expression<Func<Child1, object>> this[string fieldId]
        {
            get
            {
                if (fieldId.StartsWith("child1."))
                {
                    return AskyFieldMap.Forward<Child1, Child2>(x => x.Child2,
                        new Child2FieldMap(), fieldId.Substring("child1.".Length));
                }

                return fieldId switch
                {
                    "name" => x => x.Name,
                    _ => null,
                };
            }
        }
    }

    private class Child2FieldMap : IAskyFieldMap<Child2>
    {
        public Expression<Func<Child2, object>> this[string fieldId] => fieldId switch
        {
            "name" => x => x.Name,
            _ => null,
        };
    }

    private class Parent
    {
        public string Name { get; init; }
        public Child1 Child1 { get; init; }
    }

    private class Child1
    {
        public string Name { get; init; }
        public Child2 Child2 { get; init; }
    }

    private class Child2
    {
        public string Name { get; init; }
    }
}