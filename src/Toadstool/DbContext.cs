using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbContext : IDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;

        public DbContext()
        {
        }

        public DbContext(Func<IDbConnection> dbConnectionCreator)
            : this()
        {
            _dbConnectionProvider = new DbConnectionProvider(dbConnectionCreator);
        }

        public IDbContext WithConnection(Func<IDbConnection> dbConnectionCreator)
        {
            _dbConnectionProvider = new DbConnectionProvider(dbConnectionCreator);
            return this;
        }

        public IDbCommandBuilder Command(string commandText)
        {
            return new DbCommandBuilder()
                .WithDbConnectionProvider(_dbConnectionProvider)
                .WithCommandText(commandText);
        }

        public async Task<IDbTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _dbConnectionProvider.BeginTransactionAsync(cancellationToken);
            return _dbConnectionProvider;
        }

        public void Dispose()
        {
            _dbConnectionProvider?.Dispose();
        }
    }
}