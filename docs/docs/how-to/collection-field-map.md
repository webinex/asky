---
sidebar_position: 1
---

# Collection Field Map

Asky support reusage of `IAskyFieldMap<T>` definition

```csharp title="Parent.cs"
public class Parent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Child[] Childs { get; set; }
}
```

```csharp title="Child.cs"
public class Child
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

```csharp title="ParentFieldMap.cs
public class ParentFieldMap : IAskyFieldMap<Parent>
{
    public Expression<Func<Parent, object>> this[string fieldId] => fieldId switch
    {
        "name" => x => x.Name,
        // Define collection
        "child" => x => x.Childs,
        // Use .Select() to defined collection fields
        "child.name" => x => x.Childs.Select(c => c.Name),
        _ => null,
    };
}
```

```csharp title="ParentRepository.cs"
public class ParentRepository
{
    private readonly IAskyFieldMap<Parent> _parentFieldMap;

    // ...

    public async Task<Parent[]> GetAllParentsWithChildJimAsync()
    {
        var filterRule = FilterRule.Any(
            "child",
            FilterRule.Eq("child.name", "Jim"));

        // ...
    }
}
```
