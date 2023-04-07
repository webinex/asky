---
sidebar_position: 1
---

# Getting started

## Installation

```shell
dotnet add package Webinex.Asky
```

## Project setup

### Create model

```csharp title="Entity.cs"
public class Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
```

### Create field map

```csharp title="EntityFieldMap.cs"
private class EntityFieldMap : IAskyFieldMap<Entity>
{
    public Expression<Func<Entity, object>> this[string fieldId] => fieldId switch
    {
        "id" => x => x.Id,
        "name" => x => x.Name,
        "age" => x => x.Age,
        _ => null,
    };
}
```

```csharp title="Program.cs"
services
    .AddSingletone<IAskyFieldMap<Entity>, EntityFieldMap>();
```

### Start using it

```csharp title="EntityRepository.cs"
public class EntityRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IAskyFieldMap<Entity> _fieldMap;

    // ...

    public async Task<Entity[]> GetAllAsync(FilterRule filterRule)
    {
        return await _dbContext.Entities.AsQueryable().Where(_fieldMap, filterRule).ToArrayAsync();
    }
}
```

```csharp title="EntityService.cs"
public class EntityService
{
    private readonly EntityRepository _entityRepository;

    // ...

    public async Task<Entity[]> GetAllAdultJohnsAndJanesAsync()
    {
        var filterRule = FilterRule.And(
            FilterRule.Or(
                FilterRule.Eq("name", "John"),
                FilterRule.Eq("name", "Jane")),
            FilterRule.Gte("age", 18)
        );

        return await _entityRepository.GetAllAsync(filterRule);
    }
}
```
