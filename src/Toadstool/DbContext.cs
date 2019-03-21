using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbContext : IDbContext
    {
        public DbContext()
        {
            DataReaderDeserializer = new DefaultDataReaderDeserializer();
        }

        public IDbConnection Connection { get; private set; }
        public IDbTransaction Transaction { get; private set; }
        public IDataReaderDeserializer DataReaderDeserializer { get; private set; }

        public DbContext WithConnection(IDbConnection dbConnection)
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
            return new DbCommandBuilder()
                .WithDbContext(this)
                .WithCommandText(commandText);
        }

        public async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!(Connection is DbConnection))
            {
                throw new NotSupportedException("Connection must be DbConnection");
            }
            var dbConnection = Connection as DbConnection;
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return dbConnection;
        }

        public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Transaction == null)
            {
                Transaction = (await GetOpenConnectionAsync(cancellationToken)).BeginTransaction();
            }
            return Transaction;
        }
    }
}