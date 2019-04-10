# 🍄 Toadstool

> The Dapper alternative that nobody asked for.

[![NuGet version](https://img.shields.io/nuget/vpre/Toadstool.svg)](https://www.nuget.org/packages/Toadstool)

## Features

Query the database with ease

```csharp
var context = new DbContext()
    .WithConnection(new NpgsqlConnection("Host=localhost;Database=whatever;"));

var results = await context
    .Query("select a, b, c from bar where foo = @foo")
    .WithParameter("foo", "something")
    .ExecuteAsync()
    .AsListOf<Bar>();
```


## Building

This project targets both .NET Standard 2.0 and .NET Framework 4.5.1. Because of this, you must have .NET Framework or Mono installed (in addition to .NET Core).

On macOS and Linux build environments, to build from .NET Core you must set the `FrameworkPathOverride` environment variable.

```bash
export FrameworkPathOverride=$(dirname $(which mono))/../lib/mono/4.5/
```

The `Dockerfile` installs Mono on top of the `dotnet:2.1-sdk` image, then sets `FrameworkPathOverride` to the known location of Mono.

See https://github.com/dotnet/sdk/issues/335

---

Built with &hearts; by Calvin.

&copy; Calvin Furano