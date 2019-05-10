# 🍄 Toadstool

> The Dapper alternative that nobody asked for.

[![NuGet version](https://img.shields.io/nuget/vpre/Toadstool.svg)](https://www.nuget.org/packages/Toadstool)

## Features

* Wraps ADO.NET with a friendly, builder-pattern API.
  * Create parameters inline while building statement.
  * Map DataReader results to strongly typed list.
  * Async-first
* Makes repository pattern easier.
  * CRUD operations don't require boilerplate of creating a new connection and disposing it.
  * Transaction state is carried between different repositories.
* Fast enough.
  * Only a slight performance penalty over straight ADO.NET.

```csharp
var stockItems = await context
    .Select("stock_item_id", "stock_item_name")
    .From("warehouse.stock_items")
    .Where("supplier_id").EqualTo(2)
    .OrderBy("stock_item_name")
    .ToListAsync<StockItem>();
```

## Installation

```bash
dotnet add package Toadstool
```

--OR--

```bat
PM> Install-Package Toadstool
```

## Usage

### DbContext

The entrypoint into the Toadstool API is the `DbContext` class. Typically, only one of these should be created per database.

```csharp
var context = new DbContext();
```

The context must be able to create new instances of `IDbConnection`, so we pass it a "connection creator", which is just a lambda that returns a connection with the driver of our choosing.

```csharp
var context = new DbContext(() => new SqlConnection("Server=...")); // Use with SQL Server
var context = new DbContext(() => new NpgsqlConnection("Host=...")); // Use with PostgreSQL
```

### Statement Builder

```csharp
class Customer {
    public string FirstName { get; set; }
    public string LastName { get; set;}
    public int Age { get; set;}
}

// Do a SELECT then map the results to a list.
List<Customer> customers = await context
    .Select("first_name", "last_name", "age")
    .From("customer")
    .Where("last_name").EqualTo("Cuervo")
    .ToListAsync<Customer>();


// Execute an INSERT
int rowsAffected = await context
    InsertInto("customer", "first_name", "last_name", "age")
    .Values(
        new object[] { "Peter", "Rabbit", 100 },
        new object[] { "Santa", "Clause", 1000 },
    )
    .ExecuteNonQueryAsync();

// Execute an UPDATE
int rowsAffected = await context
    Update("customer")
    .Set(
        "first_name", "Jerry",
        "last_name", "Seinfeld"
    )
    .Where("last_name").EqualTo("Cuervo")
    .ExecuteNonQueryAsync();

// Execute a DELETE
int rowsAffected = await context
    DeleteFrom("customer")
    .Where("last_name").EqualTo("Cuervo")
    .ExecuteNonQueryAsync();
```

### Custom Statements

The statement builder shown above is optional. Statements can also be passed in as plain strings.

```csharp
class Customer {
    public string FirstName { get; set; }
    public string LastName { get; set;}
}

// Do a SELECT then map the results to a list.
List<Customer> customers = await context
    .Command(@"SELECT first_name, last_name FROM customer WHERE last_name = @lastName")
    .WithParameter("lastName", "Cuervo")
    .ToListAsync<Customer>();

// Execute an INSERT
int rowsAffected = await context
    .Command(@"INSERT INTO customer(fist_name, last_name) VALUES (@firstName, @lastName)")
    .WithParameters(new {
        firstName = "Jose",
        lastName = "Cuervo"
    })
    .ExecuteNonQueryAsync();

// Execute an UPDATE
int rowsAffected = await context
    .Command(@"UPDATE customer SET first_name = @firstName where last_name = @lastName")
    .WithParameter("firstName", "Jerry")
    .WithParameter("lastName", "Cuervo")
    .ExecuteNonQueryAsync();

// Execute a DELETE
int rowsAffected = await context
    .Command(@"DELETE FROM customer where last_name = @lastName")
    .WithParameter("lastName", "Cuervo")
    .ExecuteNonQueryAsync();

// Execute a scalar
string firstName = await context
    .Command(@"SELECT first_name FROM customer WHERE id = 42")
    .ExecuteAsync<string>();

// Execute a stored procedure
List<Customers> customers = await context
    .Command(@"spGetCustomers")
    .WithParameter("lastName", "Cuervo")
    .WithCommandType(CommandType.StoredProcedure)
    .ToListAsync<Customer>();
```

### Transactions

To start a new database transaction, call `BeginTransactionAsync` on `DbContext`. Once a transaction is begun on an instance of `DbContext`, all statements executed against that context automatically join the transaction on the same connection. Once the transaction is disposed, the context returns to normal connection pooling behavior.

```csharp
using (var transaction = await context.BeginTransactionAsync())
{
    var results1 = await context.Select("1").ExecuteScalarAsync<int>(); // Automatically joins the transaction
    var results2 = await context.Select("2").ExecuteScalarAsync<int>(); // Automatically joins the transaction

    transaction.Commit();
} 

// Transaction is disposed, context returns to normal connection pool.
var results3 = await context.Select("3").ExecuteScalarAsync<int>(); // Normal "anonymous" call (not in transaction)
```

This is useful because different repositories sharing the same `DbContext` instance can also share transactions. Say you have a service class with several repositories, each of those repositories shares the same `DbContext` instance....

```csharp
public async Task PlaceCustomerOrder(CustomerDetails customerDetails, OrderDetails orderDetails)
{
    using (var transaction = await _context.BeginTransactionAsync())
    {
        var userId = await _userRepository.CreateCustomer(customerDetails); // Automatically joins the transaction
        var orderId = await _orderRepository.CreateOrder(orderDetails); // Automatically joins the transaction
        var fulfillmentTicket = await _fulfillmentRepository.CreateFulfillmentTicket(userId, orderId); // Automatically joins the transaction

        transaction.Commit();
    }
}
```

Traditionally (with ADO or Dapper) you would have to pass around instances to your `IDbConnection` and `IDbTransaction` which is messy, or resort to using TransactionScope, which some developers believe is Dark Magic.

### Dependency Injection

If you're using a IoC container, like the one in AspNetCore, it's best to have `DbContext` be registered as "Scoped".

```csharp
services.AddScoped<IDbContext>(
    (sp) => new DbContext(() => new SqlConnection("Server=..."))
);
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

This project targets both .NET Standard 2.0 and .NET Framework 4.5.1. Because of this, you must have .NET Framework or Mono installed (in addition to .NET Core).

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



---

Built with &hearts; by Calvin.

&copy; Calvin Furano