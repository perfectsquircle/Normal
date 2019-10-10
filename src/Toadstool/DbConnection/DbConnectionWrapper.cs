
using System;
using System.Data;

namespace Toadstool
{
    internal class DbConnectionWrapper : IDbConnectionWrapper
    {
        public IDbConnection DbConnection { get; private set; }
        public IDbTransaction DbTransaction { get; private set; }

        public DbConnectionWrapper(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public DbConnectionWrapper(IDbConnection dbConnection, IDbTransaction transaction)
        {
            DbConnection = dbConnection;
            DbTransaction = transaction;
        }

        public IDbCommand CreateCommand()
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