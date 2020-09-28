# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.6.0]
### Changed
- Renamed DbCommandBuilder to CommandBuilder
- Renamed DbTransactionWrapper to Transaction
- Renamed DbConnectionWrapper to Connection

## [0.5.3]
### Added
- A \[NotMapped\] attribute to prevent a Property or Field from being mapped
- Caching column details on Table class.

## [0.5.2]
### Added
- A new SelectAsync override with a callback with the SelectBuilder

## [0.5.0]
### Changed
- InsertAsync returns model just inserted instead of rows affected.
### Removed
- CreateConnection delegate.

## [0.4.0]
### Added
- Normal.AspNetCore package
### Changed
- Renamed DbContext to Database
- Updated SDK to .NET Core 3.1
### Removed
- FastMember dependency
- Normal.Logging package
- Normal.Caching package

## [0.3.0]
### Added
- Add ToEnumerable to IDbCommandExecutor.

## [0.2.0] 
### Added
- A changelog!
- A new Constructor on DbContext that takes a "configure" callback. The callback is passes a DbContextBuilder.
- `UseConnection<T>(string connectionString)` Added to DbContextBuilder.
- `AddNormal` extension method for `IServiceCollection` for easier dependency injection.
### Changed
- Logging middleware doesn't insert newlines and tabs into log.
- IDbConnectionWrapper is no longer public
### Removed
- Static `DbContext.Create` method
- `GetOpenConnectionAsync` from `IDbContext`
- `Build` from `IDbCommandBuilder`

## [0.1.0] 
### Added
- Initial release of Normal