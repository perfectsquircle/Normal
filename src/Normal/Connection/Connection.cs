using System.Data.Common;

namespace Normal
{
    internal class Connection : IConnection
    {
        public DbConnection DbConnection { get; private set; }
        public DbTransaction DbTransaction { get; private set; }

        public Connection(DbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public Connection(DbConnection dbConnection, DbTransaction transaction)
        {
            DbConnection = dbConnection;
            DbTransaction = transaction;
        }

        public DbCommand CreateCommand()
        {
            var command = DbConnection.CreateCommand();
            command.Connection = DbConnection;
            command.Transaction = DbTransaction;
            return command;
        }

        public void Dispose()
        {
            Dispose(false);
        }

        public void Dispose(bool force)
        {
            if (force || DbTransaction == null)
            {
                DbTransaction?.Dispose();
                DbConnection?.Dispose();
            }
        }
    }
}