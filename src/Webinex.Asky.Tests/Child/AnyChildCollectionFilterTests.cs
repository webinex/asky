using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Child;

internal class AnyChildCollectionFilterTests
{
    private ParentEntity[] _data;
    private readonly ParentEntityFieldMap _fieldMap = new ParentEntityFieldMap();

    [Test]
    public void DoIt()
    {
        var result = _data.AsQueryable()
            .Where(_fieldMap, FilterRule.Any("nested", FilterRule.In("nested.value", new[] { "1-1", "99" }))).ToArray();
        result.Length.Should().Be(1);
        result.Single().Name.Should().Be("1");
        result.Single().Nested.Count.Should().Be(2);
    }

    [SetUp]
    public void SetUp()
    {
        _data = new[]
        {
            new ParentEntity
            {
                Name = "1",
                Nested = new[]
                {
                    new NestedCollectionEntity("1-1"),
                    new NestedCollectionEntity("1-2"),
                }
            }
        };
    }

    public class ParentEntity
    {
        public string Name { get; set; }
        public ICollection<NestedCollectionEntity> Nested { get; set; }
    }

    public class NestedCollectionEntity
    {
        public NestedCollectionEntity(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class ParentEntityFieldMap : IAskyFieldMap<ParentEntity>
    {
        public Expression<Func<ParentEntity, object>> this[string fieldId]
        {
            get
            {
                return fieldId switch
                {
                    "name" => x => x.Name,
                    "nested" => x => x.Nested,
                    "nested.value" => x => x.Nested.Select(v => v.Value),
                    _ => null,
                };
            }
        }
    }
}