# Normal.AspNetCore

> A Normal extension that adds caching, logging, and DI support.

[![NuGet version](https://img.shields.io/nuget/vpre/Normal.Caching.svg)](https://www.nuget.org/packages/Normal.Caching)

## Installation

```bash
dotnet add package Normal.Caching
```

### Dependency Injection

If you're using a IoC container, like the one in AspNetCore, call the `AddNormal` method to inject a scoped `IDatabase`.

```csharp
services.AddNormal((sp, c) =>
{
    c.UseConnection<NpgsqlConnection>(connectionString);
});
```

Then in your classes, you can use constructor injection to get a reference to IDatabase.

```csharp
using System.Threading.Tasks;
using Normal;

class CustomerRepository 
{
    private readonly IDatabase _database;

    public CustomerRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task<Customer> GetCustomer(int id)
    {
        return await _database
            .Select("first_name", "last_name", "age")
            .From("customer")
            .Where("customer_id").EqualTo(id)
            .FirstOrDefaultAsync<Customer>();
    }
}
```

### Caching

```csharp
services.AddNormal((sp, c) =>
{
    c.UseConnection<NpgsqlConnection>(connectionString);
    c.UseCaching(sp.GetService<IMemoryCache>())
});

// Now you can cache queries for a given timespan.

var results = await database
    .Select("stock_item_id", "stock_item_name")
    .From("warehouse.stock_items")
    .CacheFor(TimeSpan.FromMinutes(5))
    .ToListAsync<StockItem>();

```

### Logging

```csharp
services.AddNormal((sp, c) =>
{
    c.UseConnection<NpgsqlConnection>(connectionString);
    c.UseLogging(sp.GetService<ILogger<Database>>());
});

// Now every query will be logged at info level

var results = await database
    .Select("stock_item_id", "stock_item_name")
    .From("warehouse.stock_items")
    .Where("supplier_id").EqualTo(2)
    .And("tax_rate").EqualTo(15.0)
    .OrderBy("stock_item_id")
    .ToListAsync<StockItem>();

/**
[9:10:11 INF] query: SELECT stock_item_id, stock_item_name FROM warehouse.stock_items WHERE supplier_id = @normal_1 AND tax_rate = @normal_2 ORDER BY stock_item_id
        parameters: {"normal_1": 2, "normal_2": 15}
        elapsed: 5ms
**/
```

Built with &hearts; by Calvin.

&copy; Calvin Furano