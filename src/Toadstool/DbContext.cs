using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbContext : IDbContext
    {
        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }
        public IDataReaderDeserializer DataReaderDeserializer { get; private set; }

        public DbContext()
        {
            DataReaderDeserializer = new DefaultDataReaderDeserializer();
        }

        public DbContext WithConnection(DbConnection dbConnection)
        {
            Connection = dbConnection;
            return this;
        }

        public DbContext WithDataRowDeserializer(IDataReaderDeserializer dataReaderDeserializer)
        {
            DataReaderDeserializer = dataReaderDeserializer;
            return this;
        }

        public DbCommandBuilder Query(string commandText)
        {
            return new DbCommandBuilder(this)
                .WithCommandText(commandText);
        }

        public async Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var sqlConnection = Connection;
            await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return sqlConnection;
        }

        public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Transaction == null)
            {
                Transaction = (await GetOpenConnectionAsync(cancellationToken)).BeginTransaction();
            }
            return Transaction;
        }
    }
}