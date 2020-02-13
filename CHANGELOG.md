# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.4.0]
### Changed
- Renamed DbContext to Database

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