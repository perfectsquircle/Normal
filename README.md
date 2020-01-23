# Normal

[![NuGet version](https://img.shields.io/nuget/vpre/Normal.svg)](https://www.nuget.org/packages/Normal)

## Introduction

Normal is a small and extensible [ORM](https://en.wikipedia.org/wiki/Object-relational_mapping) for .NET available as a NuGet package. 

- [Normal](#normal)
  - [Introduction](#introduction)
  - [Installation](#installation)
  - [Usage](#usage)
    - [DbContext](#dbcontext)
    - [Statement Builder](#statement-builder)
    - [CRUD statements](#crud-statements)
    - [Custom Commands](#custom-commands)
    - [Middleware](#middleware)
    - [Custom Middleware](#custom-middleware)
    - [Transactions](#transactions)
    - [Dependency Injection](#dependency-injection)
  - [Building](#building)
  - [Testing](#testing)

## Installation

| Platform       | Minimum Version |
| :------------- | :-------------- |
| .NET Standard  | 2.0             |
| .NET Framework | 4.6.1           |

```bash
dotnet add package Normal
```

--OR--

```bat
PM> Install-Package Normal
```

## Usage

### DbContext

The entrypoint into the Normal API is the `DbContext` class. Typically, only one of these should be created per database in your application lifetime (or HTTP Request lifetime.) A DbContext is intended to be injected into and shared amongst other classes.

The context must be able to create new instances of `IDbConnection`, so we pass it a `CreateConnection` delegate, which is just a function that returns a new connection with the driver of our choosing.

```csharp
// Use with SQL Server
var context = new DbContext(() => new SqlConnection("Server=...")); 
// Use with PostgreSQL
var context = new DbContext(() => new NpgsqlConnection("Host=...")); 
```

### Statement Builder

For very simple queries, you can use the inline statement builder for `SELECT`, `INSERT`, `UPDATE`, and `DELETE`. This can map the results to any POCO class.

```csharp
class Customer {
    public string FirstName { get; set; }
    public string LastName { get; set;}
    public int Age { get; set;}
}

// Do a SELECT then map the results to a list.
IList<Customer> customers = await context
    .Select("first_name", "last_name", "age")
    .From("customer")
    .Where("last_name").EqualTo("Cuervo")
    .ToListAsync<Customer>();

// Do a SELECT then grab the first result.
Customer customer = await context
    .Select("first_name", "last_name", "age")
    .From("customer")
    .Where("customer_id").EqualTo(777)
    .FirstOrDefaultAsync<Customer>();

// Execute an INSERT
int rowsAffected = await context
    .InsertInto("customer")
    .Columns("first_name", "last_name", "age")
    .Values("Peter", "Rabbit", 100)
    .Values("Santa", "Clause", 1000)
    .Execute();

// Execute an UPDATE
int rowsAffected = await context
    .Update("customer")
    .Set("first_name").EqualTo("Jerry")
    .Set("last_name").EqualTo("Seinfeld")
    .Where("last_name").EqualTo("Cuervo")
    .Execute();

// Execute a DELETE
int rowsAffected = await context
    .DeleteFrom("customer")
    .Where("last_name").EqualTo("Cuervo")
    .Execute();
```

### CRUD statements

Simple crud operations can be executed using some convenience methods on `DbContext`. To use these, it's recommended that you annotate your models with the `Table`, `PrimaryKey` and `Column` annotations. If the annotations are omitted, Normal will use the class name as the table name, and the field names as the column names.

```csharp
[Table("warehouse.stock_items")]
public class StockItem
{
    [PrimaryKey]
    [Column("stock_item_id")]
    public int StockItemID { get; set; }

    [Column("stock_item_name")]
    public string StockItemName { get; set; }

    [Column("supplier_id")]
    public int SupplierId { get; set; }
}

// SELECT all rows from stock_items and map them to a list of StockItem
var results = await context.SelectAsync<StockItem>();

// SELECT the row where stock_item_id = 1 and map it to a StockItem (or null.)
var result = await context.SelectAsync<StockItem>(1);

// INSERT a row into stock_items, using the fields on the stockItem model.
var rowsAffected = await context.InsertAsync<StockItem>(stockItem);

// INSERT a row in stock_items, using the fields on the stockItem model.
var rowsAffected = await context.UpdateAsync<StockItem>(stockItem);

// DELETE a row from stock_items
var rowsAffected = await context.DeleteAsync<StockItemAnnotated>(stockItem);
```

### Custom Commands

For more complicated queries, commands can be created from a string, an embedded resource, or a file.

```csharp
// Create a command from a string, add a parameter, and map results to a list.
var customers = await context
    .CreateCommand(@"SELECT first_name, last_name FROM customer WHERE last_name = @lastName")
    .WithParameter("lastName", "Cuervo")
    .ToListAsync<Customer>();

// Normal will load the resource from the calling assembly. 
context.CreateCommandFromResource("My.Assembly.GetCustomers.sql");

// The assembly name may be omitted. Normal will load the first resource that ends with the given string.
context.CreateCommandFromResource("GetCustomers.sql");

// Optionally, you may pass an assembly to load the embedded resource from
context.CreateCommandFromResource("GetCustomers.sql", myAssembly);
    
// Also, you can load a command from any file.
context.CreateCommandFromFile("/path/to/sql/GetCustomers.sql");
```

### Middleware

Middleware can be installed on a `DbContext` that will execute on every database request. 

Existing middleware:
* [Normal.Logging](./src/Normal.Logging/README.md)
* [Normal.Caching](./src/Normal.Caching/README.md)

Middleware is added using the `DbContext` builder constructor. 

```csharp
var context = new DbContext(c =>
{
    c.UseConnection<NpgsqlConnection>(connectionString); 
    c.UseLogging(logger); // Add logging middleware
});

// Now every query will be logged at info level

var results = await context
    .Select("stock_item_id", "stock_item_name")
    .From("warehouse.stock_items")
    .Where("supplier_id").EqualTo(2)
    .And("tax_rate").EqualTo(15.0)
    .OrderBy("stock_item_id")
    .ToListAsync<StockItem>();

/**
[9:10:11 INF] query: SELECT stock_item_id, stock_item_name FROM warehouse.stock_items WHERE supplier_id = @normal_1 AND tax_rate = @normal_2 ORDER BY stock_item_id parameters: {"normal_1": 2, "normal_2": 15} elapsed: 5ms
**/
```

### Custom Middleware

Normal is extensible, and you can write your own middleware!

```csharp
public class AwesomeHandler : DelegatingHandler
{
    public override async Task<int> ExecuteNonQueryAsync(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
    {
        // Do stuff before non-query
        var rowsAffected = await InnerHandler.ExecuteNonQueryAsync(commandBuilder, cancellationToken);
        // Do stuff after non-query
        return rowsAffected;
    }

    public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
    {
        // Do stuff before query
        var results = await InnerHandler.ExecuteReaderAsync<T>(commandBuilder, cancellationToken);
        // Do stuff after query
        return results;
    }

    public override async Task<T> ExecuteScalarAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
    {
        // Do stuff before scalar
        var result = await InnerHandler.ExecuteScalarAsync<T>(commandBuilder, cancellationToken);
        // Do stuff after scalar
        return result;
    }
}
```

You can install this on `DbContext` by using `new DbContext`.

```csharp
var context = new DbContext(c =>
{
    c.UseConnection(connection);
    c.UseDelegatingHandler(new AwesomeHandler()); // Add custom middleware.
});
```

Middleware is executed in the order that it was added. For example, if you added three DelegatingHandlers...

```csharp
var context = new DbContext(c =>
{
    c.UseDelegatingHandler(new A())
    c.UseDelegatingHandler(new B())
    c.UseDelegatingHandler(new C())
});
```

Then for every database query, the middlewares are executed in order in a nested fashion.

```
A
  B
    C
      BaseHandler
    C
  B
A
```

### Transactions

To start a new database transaction, call `BeginTransaction` on `DbContext`. Once a transaction is begun on an instance of `DbContext`, all statements executed against that context automatically join the transaction on the same connection. Once the transaction is disposed, the context returns to connection pooling behavior.

This is useful because different repositories sharing the same `DbContext` instance can also share transactions. Say you have a service class with several repositories. Because you're using dependency injection, each of those repositories shares the same `DbContext` instance...

```csharp
private readonly IDbContext _context;

public async Task PlaceCustomerOrder(CustomerDetails customerDetails, OrderDetails orderDetails)
{
    using (var transaction = await _context.BeginTransactionAsync())
    {
        // Automatically joins the transaction
        var userId = await _userRepository.CreateCustomer(customerDetails);
        // Automatically joins the transaction
        var orderId = await _orderRepository.CreateOrder(orderDetails); 
        // Automatically joins the transaction
        var fulfillmentTicket = await _fulfillmentRepository.CreateFulfillmentTicket(userId, orderId);

        transaction.Commit();
    }
}
```

### Dependency Injection

If you're using a IoC container, like the one in AspNetCore, call the `AddNormal` method to inject a scoped `IDbContext`.

```csharp
services.AddNormal((sp, c) =>
{
    c.UseConnection<NpgsqlConnection>(connectionString);
    c.UseLogging(sp.GetService<ILogger<DbContext>>());
});
```

Then in your classes, you can use constructor injection to get a reference to IDbContext.

```csharp
using System.Threading.Tasks;
using Normal;

class CustomerDataAccess 
{
    private readonly IDbContext _context;

    public CustomerDataAccess(IDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> GetCustomer(int id)
    {
        return await _context
            .Select("first_name", "last_name", "age")
            .From("customer")
            .Where("customer_id").EqualTo(id)
            .FirstOrDefaultAsync<Customer>();
    }
}
```

## Building

Prerequisites:
* .NET Core SDK 2.1 
* Mono or .NET Framework
* Gnu Make

To build the NuGet package.

```bash
make pack
```

This project targets both .NET Standard 2.0 and .NET Framework 4.6.1. Because of this, you must have .NET Framework or Mono installed (in addition to .NET Core).

On macOS and Linux build environments, to build from .NET Core you must set the `FrameworkPathOverride` environment variable.

```bash
export FrameworkPathOverride=$(dirname $(which mono))/../lib/mono/4.5/
```

See https://github.com/dotnet/sdk/issues/335

## Testing

Prerequisites:
* .NET Core SDK 2.1
* Gnu Make
* Docker
* Bash

Running the integration tests requires having a recent version of Docker installed. Two database servers (PostgreSQL and SQL Server) will be brought up with 

```bash
make databases
```

After the servers are up and the databases are restored, the tests can be run.

```bash
make test
```

To bring down the servers and clean up the backup files,

```bash
make clean-databases
```


Built with &hearts; by Calvin.

&copy; Calvin Furano