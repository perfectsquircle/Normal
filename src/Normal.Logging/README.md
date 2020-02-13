# Normal.Logging

> A Normal middleware for logging

[![NuGet version](https://img.shields.io/nuget/vpre/Normal.Logging.svg)](https://www.nuget.org/packages/Normal.Logging)

## Installation

```bash
dotnet add package Normal.Logging
```

### Usage

```csharp
var database = new DatabaseBuilder()
    .WithCreateConnection(() => new NpgsqlConnection("..."))
    .WithLogging(logger) // Add logging middleware.
    .Build();

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