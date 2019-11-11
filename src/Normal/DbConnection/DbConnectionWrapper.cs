using System.Data.Common;

namespace Normal
{
    internal class DbConnectionWrapper : IDbConnectionWrapper
    {
        public DbConnection DbConnection { get; private set; }
        public DbTransaction DbTransaction { get; private set; }

        public DbConnectionWrapper(DbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public DbConnectionWrapper(DbConnection dbConnection, DbTransaction transaction)
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