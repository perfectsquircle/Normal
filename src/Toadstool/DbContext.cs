using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbContext : IDbContext
    {
        public DbConnection Connection { get; }
        public DbTransaction Transaction { get; private set; }

        public DbContext(DbConnection dbConnection)
        {
            Connection = dbConnection;
        }

        public DbCommandBuilder Query(string commandText)
        {
            return new DbCommandBuilder(this)
                .WithCommandText(commandText);
        }

        public Task<DbConnection> GetOpenConnectionAsync() => GetOpenConnectionAsync(CancellationToken.None);
        public async Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            var sqlConnection = Connection;
            await sqlConnection.OpenAsync(cancellationToken);
            return sqlConnection;
        }

        public Task<DbTransaction> BeginTransactionAsync() => BeginTransactionAsync(CancellationToken.None);
        public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (Transaction == null)
            {
                Transaction = (await GetOpenConnectionAsync()).BeginTransaction();
            }
            return Transaction;
        }
    }
}