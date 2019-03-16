using System.Data;
using System.Threading;

namespace Toadstool
{
    public abstract class DbContext : IDbContext
    {
        protected abstract ThreadLocal<IDbConnection> Connection { get; }
        public DbCommandBuilder Query(string commandText)
        {
            return new DbCommandBuilder(this)
                .WithCommandText(commandText);
        }

        public IDbConnection GetOpenConnection()
        {
            var sqlConnection = Connection.Value;
            sqlConnection.Open();
            return sqlConnection;
        }

        public IDbTransaction GetTransaction()
        {
            return GetOpenConnection().BeginTransaction();
        }
    }
}