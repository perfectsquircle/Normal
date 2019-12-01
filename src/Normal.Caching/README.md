# Normal.Caching

> A Normal middleware for caching

[![NuGet version](https://img.shields.io/nuget/vpre/Normal.Caching.svg)](https://www.nuget.org/packages/Normal.Caching)

## Installation

```bash
dotnet add package Normal.Caching
```

### Usage

```csharp
var context = new DbContext(c =>
{
    c.UseConnection(connection); 
    c.UseCaching(memoryCache); // Add caching middleware.
});

// Now you can cache queries for a given timespan.

var results = await context
    .Select("stock_item_id", "stock_item_name")
    .From("warehouse.stock_items")
    .CacheFor(TimeSpan.FromMinutes(5))
    .ToListAsync<StockItem>();

```

Built with &hearts; by Calvin.

&copy; Calvin Furano