using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Webinex.Asky.Tests.Child;

internal class AnyChildCollectionFilterTests
{
    [Test]
    public void WhenNestedString_ShouldWorkCorrectly()
    {
        var fieldMap = new ParentEntityFieldMap<string>();
        var data = new[]
        {
            new ParentEntity<string>("1", new[]
            {
                new NestedCollectionEntity<string>("1-1"),
                new NestedCollectionEntity<string>("1-2"),
            }),
        };
        var result = data.AsQueryable()
            .Where(fieldMap, FilterRule.Any("nested", FilterRule.In("nested.value", new[] { "1-1", "99" })))
            .ToArray();
        result.Length.Should().Be(1);
        result.Single().Name.Should().Be("1");
        result.Single().Nested.Count.Should().Be(2);
    }

    [Test]
    public void WhenNestedGuid_ShouldWorkCorrectly()
    {
        var fieldMap = new ParentEntityFieldMap<Guid>();
        var data = new[]
        {
            new ParentEntity<Guid>("1", new[]
            {
                new NestedCollectionEntity<Guid>(Guid.Parse("B380EE6E-1CB2-4485-B616-A89D238F3244")),
                new NestedCollectionEntity<Guid>(Guid.NewGuid()),
            }),
        };
        var result = data.AsQueryable()
            .Where(fieldMap,
                FilterRule.Any("nested",
                    FilterRule.In("nested.value",
                        new[] { Guid.Parse("B380EE6E-1CB2-4485-B616-A89D238F3244"), Guid.NewGuid() })))
            .ToArray();
        result.Length.Should().Be(1);
        result.Single().Name.Should().Be("1");
        result.Single().Nested.Count.Should().Be(2);
    }

    [Test]
    public void WhenNestedGuid_UsingSQL_ShouldWorkCorrectly()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection).Options;
        using var dbContext = new TestDbContext(options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        dbContext.Set<ParentEntity<Guid>>().AddRange(new ParentEntity<Guid>("1", new[]
        {
            new NestedCollectionEntity<Guid>(Guid.Parse("B380EE6E-1CB2-4485-B616-A89D238F3244")),
            new NestedCollectionEntity<Guid>(Guid.NewGuid()),
        }));
        dbContext.SaveChanges();

        var fieldMap = new ParentEntityFieldMap<Guid>();
        var result = dbContext.Set<ParentEntity<Guid>>().AsQueryable()
            .Where(fieldMap,
                FilterRule.Any("nested",
                    FilterRule.In("nested.value",
                        new[] { Guid.Parse("B380EE6E-1CB2-4485-B616-A89D238F3244"), Guid.NewGuid() })))
            .ToArray();
        result.Length.Should().Be(1);
        result.Single().Name.Should().Be("1");
        result.Single().Nested.Count.Should().Be(2);
    }

    private class ParentEntityFieldMap<T> : IAskyFieldMap<ParentEntity<T>>
    {
        public Expression<Func<ParentEntity<T>, object>> this[string fieldId] => fieldId switch
        {
            "name" => x => x.Name,
            "nested" => x => x.Nested,
            "nested.value" => x => x.Nested.Select(v => v.Value),
            _ => null,
        };
    }
}