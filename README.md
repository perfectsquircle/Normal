# 🍄 Toadstool

> The Dapper alternative that nobody asked for.

[![NuGet version](https://img.shields.io/nuget/vpre/Toadstool.svg)](https://www.nuget.org/packages/Toadstool)

## Features

* Wraps ADO.NET with an async-first and fluent API.
* Maps DataReader results to a strongly typed list of objects.
* CRUD operations don't require boilerplate of creating a new connection, opening, and disposing it.
* Transaction state can be carried between different repositories.

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

The entrypoint into the Toadstool API is the `DbContext` class. Typically, only one of these should be created per database in your application lifetime (or HTTP Request lifetime.)

```csharp
var context = new DbContext();
```

The context must be able to create new instances of `IDbConnection`, so we pass it a "connection factory", which is just a function that returns a new connection with the driver of our choosing.

```csharp
// Use with SQL Server
var context = new DbContext(() => new SqlConnection("Server=...")); 
// Use with PostgreSQL
var context = new DbContext(() => new NpgsqlConnection("Host=...")); 
```

### Statement Builder

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

### Custom Statements

The statement builder shown above is optional. Statements can also be passed in as plain strings.

```csharp
class Customer {
    public string FirstName { get; set; }
    public string LastName { get; set;}
}

// Do a SELECT then map the results to a list.
IList<Customer> customers = await context
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
    .Execute();

// Execute an UPDATE
int rowsAffected = await context
    .Command(@"UPDATE customer SET first_name = @firstName where last_name = @lastName")
    .WithParameter("firstName", "Jerry")
    .WithParameter("lastName", "Cuervo")
    .Execute();

// Execute a DELETE
int rowsAffected = await context
    .Command(@"DELETE FROM customer where last_name = @lastName")
    .WithParameter("lastName", "Cuervo")
    .Execute();

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
    // Automatically joins the transaction
    var results1 = await context.Select("1").ExecuteAsync<int>(); 
    // Automatically joins the transaction
    var results2 = await context.Select("2").ExecuteAsync<int>();

    transaction.Commit();
} 

// Transaction is disposed, context returns to normal connection pool.
var results3 = await context.Select("3").ExecuteAsync<int>(); // Normal "anonymous" call (not in transaction)
```

**Please note, `DbContext` is not thread-safe. Instances should not be shared across threads.**

This is useful because different repositories sharing the same `DbContext` instance can also share transactions. Say you have a service class with several repositories. Because you're using dependency injection, each of those repositories shares the same `DbContext` instance....

```csharp
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

Traditionally (with ADO or Dapper) you would have to pass around instances to your `IDbConnection` and `IDbTransaction` which is messy, rewrite a boilerplate connection provider class every time, or resort to using TransactionScope, which some developers believe is Dark Magic.

### Dependency Injection

If you're using a IoC container, like the one in AspNetCore, it's best to have `DbContext` be registered as "Scoped".

```csharp
services.AddScoped<IDbContext>(
    (sp) => new DbContext(() => new SqlConnection("Server=..."))
);
```

This way, everything in the same request scope can share transactions.

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

## Motivation

I was curious about how Dapper was implemented, so I went to read the source code. I was horrified at the [method sprawl](https://github.com/StackExchange/Dapper/blob/master/Dapper/SqlMapper.Async.cs) and the classes thousands of lines long.

To better understand the problem space, I set out to write a clone. Using the builder-pattern, I was quickly able to match the core Dapper API `.Query()` in a much tighter code base.

I understand that Dapper has grown organically over the years to solve a thousand edge cases that I don't even know about. It's also faster than Toadstool ever will be. However this was a good learning experience.

Additionally, I wanted to solve transaction management without the dark magic of `TransactionScope`. When you're implementing the repository pattern in raw ADO.NET or Dapper, there isn't any default way to share a transaction across multiple repository methods, or multiple repositories for that matter. 

---

Built with &hearts; by Calvin.

&copy; Calvin Furano