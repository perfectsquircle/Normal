using System.Data;

namespace Toadstool
{
    internal class DbConnectionWrapper : IDbConnectionWrapper
    {
        public DbConnectionWrapper(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public IDbConnection DbConnection { get; private set; }

        public virtual IDbCommand CreateCommand()
        {
            var command = DbConnection.CreateCommand();
            command.Connection = DbConnection;
            return command;
        }

        public virtual void Dispose()
        {
            DbConnection?.Dispose();
        }
    }
}