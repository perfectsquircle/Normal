# Normal

[![NuGet version](https://img.shields.io/nuget/vpre/Normal.svg)](https://www.nuget.org/packages/Normal)

## Introduction

Normal is a small and extensible [ORM](https://en.wikipedia.org/wiki/Object-relational_mapping) for .NET available as a NuGet package. 

It has no third-party dependencies, and can be dropped into an existing project.

- [Normal](#normal)
  - [Introduction](#introduction)
  - [Installation](#installation)
  - [Usage](#usage)
    - [Database Class](#database-class)
    - [Statement Builder](#statement-builder)
    - [CRUD statements](#crud-statements)
    - [Custom Commands](#custom-commands)
    - [Custom Middleware](#custom-middleware)
    - [Transactions](#transactions)
    - [AspNetCore](#aspnetcore)
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

### Database Class

The entrypoint into the Normal API is the `Database` class. Typically, only one of these should be created per database in your application lifetime (or HTTP Request lifetime.) A `Database` is intended to be injected into and shared amongst other classes.



```csharp
// Use with SQL Server
var database = Database.WithConnection<SqlConnection>("Server=..."); 
// Use with PostgreSQL
var database = Database.WithConnection<NpgsqlConnection>("Host=..."); 
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
IList<Customer> customers = await database
    .Select("first_name", "last_name", "age")
    .From("customer")
    .Where("last_name").EqualTo("Cuervo")
    .ToListAsync<Customer>();

// Do a SELECT then grab the first result.
Customer customer = await database
    .Select("first_name", "last_name", "age")
    .From("customer")
    .Where("customer_id").EqualTo(777)
    .FirstOrDefaultAsync<Customer>();

// Execute an INSERT
int rowsAffected = await database
    .InsertInto("customer")
    .Columns("first_name", "last_name", "age")
    .Values("Peter", "Rabbit", 100)
    .Values("Santa", "Clause", 1000)
    .Execute();

// Execute an UPDATE
int rowsAffected = await database
    .Update("customer")
    .Set("first_name").EqualTo("Jerry")
    .Set("last_name").EqualTo("Seinfeld")
    .Where("last_name").EqualTo("Cuervo")
    .Execute();

// Execute a DELETE
int rowsAffected = await database
    .DeleteFrom("customer")
    .Where("last_name").EqualTo("Cuervo")
    .Execute();
```

### CRUD statements

Simple crud operations can be executed using some convenience methods on `Database`. To use these, it's recommended that you annotate your models with the `Table`, `PrimaryKey` and `Column` annotations. If the annotations are omitted, Normal will use the class name as the table name, and the field names as the column names.

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

    [NotMapped]
    public string ComputedProperty { get; set; }
}

// SELECT all rows from stock_items and map them to a list of StockItem
var stockItems = await database.SelectAll<StockItem>().ToListAsync();

// SELECT the row where stock_item_id = 1 and map it to a StockItem (or null.)
var stockItem = await database.SelectAsync<StockItem>(1);

// SELECT the rows where stock_item_name = "USB missile launcher (Green)" and map it to a list of StockItem
var results = await database
    .SelectAll<StockItem>()
    .Where("stock_item_name").EqualTo("USB missile launcher (Green)");

// INSERT a row into stock_items, using the fields on the stockItem model.
var insertedStockItem = await database.InsertAsync<StockItem>(stockItem);

// INSERT a row in stock_items, using the fields on the stockItem model.
var updatedStockItem = await database.UpdateAsync<StockItem>(stockItem);

// DELETE a row from stock_items
var rowsAffected = await database.DeleteAsync<StockItem>(stockItem);
```

### Custom Commands

For more complicated queries, commands can be created from a string, an embedded resource, or a file.

```csharp
// Create a command from a string, add a parameter, and map results to a list.
var customers = await database
    .CreateCommand(@"SELECT first_name, last_name FROM customer WHERE last_name = @lastName")
    .WithParameter("lastName", "Cuervo")
    .ToListAsync<Customer>();

// Normal will load the resource from the calling assembly. 
database.CreateCommandFromResource("My.Assembly.GetCustomers.sql");

// The assembly name may be omitted. Normal will load the first resource that ends with the given string.
database.CreateCommandFromResource("GetCustomers.sql");

// Optionally, you may pass an assembly to load the embedded resource from
database.CreateCommandFromResource("GetCustomers.sql", myAssembly);
    
// Also, you can load a command from any file.
database.CreateCommandFromFile("/path/to/sql/GetCustomers.sql");
```

### Custom Middleware

Normal is extensible, and you can write your own middleware!

```csharp
public class AwesomeHandler : DelegatingHandler
{
    public override async Task<int> ExecuteNonQueryAsync(ICommandBuilder commandBuilder, CancellationToken cancellationToken)
    {
        // Do stuff before non-query
        var rowsAffected = await InnerHandler.ExecuteNonQueryAsync(commandBuilder, cancellationToken);
        // Do stuff after non-query
        return rowsAffected;
    }

    public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(ICommandBuilder commandBuilder, CancellationToken cancellationToken)
    {
        // Do stuff before query
        var results = await InnerHandler.ExecuteReaderAsync<T>(commandBuilder, cancellationToken);
        // Do stuff after query
        return results;
    }

    public override async Task<T> ExecuteScalarAsync<T>(ICommandBuilder commandBuilder, CancellationToken cancellationToken)
    {
        // Do stuff before scalar
        var result = await InnerHandler.ExecuteScalarAsync<T>(commandBuilder, cancellationToken);
        // Do stuff after scalar
        return result;
    }
}
```

You can install this on `Database` by using `new Database`.

```csharp
var database = new Database(c =>
{
    c.UseConnection(connection);
    c.UseDelegatingHandler(new AwesomeHandler()); // Add custom middleware.
});
```

Middleware is executed in the order that it was added. For example, if you added three DelegatingHandlers...

```csharp
var database = new Database(c =>
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

To start a new database transaction, call `BeginTransaction` on `Database`. Once a transaction is begun on an instance of `Database`, all statements executed against that database automatically join the transaction on the same connection. Once the transaction is disposed, the database returns to connection pooling behavior.

This is useful because different repositories sharing the same `Database` instance can also share transactions. Say you have a service class with several repositories. Because you're using dependency injection, each of those repositories shares the same `Database` instance...

```csharp
private readonly IDatabase _database;

public async Task PlaceCustomerOrder(CustomerDetails customerDetails, OrderDetails orderDetails)
{
    using (var transaction = await _database.BeginTransactionAsync())
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

### AspNetCore

There is an AspNetCore plugin that adds caching, logging, and DI support. See [Normal.AspNetCore](src/Normal.AspNetCore/README.md).

## Building

Prerequisites:
* .NET Core SDK 3.1 
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
* .NET Core SDK 3.1
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