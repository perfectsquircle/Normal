using System;
using System.Data;

namespace Toadstool
{
    internal class DbConnectionWrapper : IDbConnectionWrapper
    {
        private IDbConnection _dbConnection;
        private IDbTransaction _dbTransaction;
        private bool _transactionIsComplete;

        public DbConnectionWrapper(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            _transactionIsComplete = true;
        }

        public DbConnectionWrapper(IDbConnection dbConnection, IDbTransaction transaction)
        {
            _dbConnection = dbConnection;
            _dbTransaction = transaction;
        }

        public CommandBehavior CommandBehavior => _transactionIsComplete ? CommandBehavior.CloseConnection : CommandBehavior.Default;

        public IDbCommand CreateCommand()
        {
            var command = _dbConnection.CreateCommand();
            command.Connection = _dbConnection;
            command.Transaction = _dbTransaction;
            return command;
        }

        public void Commit()
        {
            _transactionIsComplete = true;
            _dbTransaction?.Commit();
        }

        public void Rollback()
        {
            _transactionIsComplete = true;
            _dbTransaction?.Rollback();
        }

        public void Dispose()
        {
            Dispose(false);
        }

        public void Dispose(bool force)
        {
            if (force || _transactionIsComplete)
            {
                _dbTransaction?.Dispose();
                _dbConnection?.Dispose();
            }
        }
    }
}