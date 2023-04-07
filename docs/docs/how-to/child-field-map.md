---
sidebar_position: 2
---

# Child Field Map

Asky support reusage of `IAskyFieldMap<T>` definition

```csharp title="Human.cs"
public class Human
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

```csharp title="Car.cs"
public class Car
{
    public Guid Id { get; set; }
    public Human Owner { get; set; }
    public string Model { get; set; }
}
```

```csharp title="HumanFieldMap.cs
public class HumanFieldMap : IAskyFieldMap<Human>
{
    public Expression<Func<Human, object>> this[string fieldId] => fieldId switch
    {
        "name" => x => x.Name,
        _ => null,
    };
}
```


```csharp title="CarFieldMap.cs"
public class CarFieldMap : IAskyFieldMap<Car>
{
    private readonly IAskyFieldMap<Human> _humanFieldMap;

    // ...

    public Expression<Func<Car, object>> this[string fieldId]
    {
        get
        {
            if (fieldId.StartsWith("owner."))
            {
                return AskyFieldMap.Forward<Car, Human>(x => x.Owner,
                    _humanFieldMap, fieldId.Substring("owner.".Length));
            }

            return fieldId switch
            {
                "model" => x => x.Model,
                _ => null,
            };
        }
    }
}
```

```csharp title="CarRepository.cs"
public class CarRepository
{
    private readonly IAskyFieldMap<Car> _carFieldMap;

    // ...

    public async Task<Car[]> GetAllCamryOwnedByJohnsAsync()
    {
        var filterRule = FilterRule.And(
            FilterRule.Eq("owner.name", "John"),
            FilterRule.Eq("model", "Camry"));

        // ...
    }
}
```
