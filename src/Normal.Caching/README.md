# Normal.Caching

> A Normal middleware for caching

[![NuGet version](https://img.shields.io/nuget/vpre/Normal.Caching.svg)](https://www.nuget.org/packages/Normal.Caching)

## Installation

```bash
dotnet add package Normal.Caching
```

### Usage

```csharp
var context = new DbContextBuilder()
    .WithCreateConnection(() => new NpgsqlConnection("..."))
    .WithCaching(memoryCache) // Add caching middleware.
    .Build();

// Now ...

var results = await context
    .Select("stock_item_id", "stock_item_name")
    .From("warehouse.stock_items")
    .Where("supplier_id").EqualTo(2)
    .And("tax_rate").EqualTo(15.0)
    .OrderBy("stock_item_id")
    .CacheFor(TimeSpan.FromMinutes(5))
    .ToListAsync<StockItem>();

```

Built with &hearts; by Calvin.

&copy; Calvin Furano