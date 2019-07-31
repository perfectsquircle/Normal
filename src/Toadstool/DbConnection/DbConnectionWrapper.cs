using System;
using System.Data;

namespace Toadstool
{
    internal class DbConnectionWrapper : IDbConnectionWrapper
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _dbTransaction;

        public DbConnectionWrapper(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public DbConnectionWrapper(IDbConnection dbConnection, IDbTransaction transaction)
        {
            _dbConnection = dbConnection;
            _dbTransaction = transaction;
        }

        public CommandBehavior CommandBehavior => _dbTransaction == null ? CommandBehavior.CloseConnection : CommandBehavior.Default;

        public IDbCommand CreateCommand()
        {
            var command = _dbConnection.CreateCommand();
            command.Connection = _dbConnection;
            command.Transaction = _dbTransaction;
            return command;
        }

        public void Dispose()
        {
            Dispose(false);
        }

        public void Dispose(bool force)
        {
            if (force || _dbTransaction == null)
            {
                _dbTransaction?.Dispose();
                _dbConnection?.Dispose();
            }
        }
    }
}