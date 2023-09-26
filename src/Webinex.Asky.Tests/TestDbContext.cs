using System;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Asky.Tests;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParentEntity<Guid>>();
        modelBuilder.Entity<NestedCollectionEntity<Guid>>();
        base.OnModelCreating(modelBuilder);
    }
}