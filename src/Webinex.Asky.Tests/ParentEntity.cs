using System;
using System.Collections.Generic;

namespace Webinex.Asky.Tests;

public class ParentEntity<T>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<NestedCollectionEntity<T>> Nested { get; set; }

    protected ParentEntity()
    {
    }

    public ParentEntity(string name, ICollection<NestedCollectionEntity<T>> nested)
    {
        Name = name;
        Nested = nested;
    }
}

public class NestedCollectionEntity<T>
{
    public Guid Id { get; set; }
    public T Value { get; set; }

    protected NestedCollectionEntity()
    {
    }

    public NestedCollectionEntity(T value)
    {
        Value = value;
    }
}