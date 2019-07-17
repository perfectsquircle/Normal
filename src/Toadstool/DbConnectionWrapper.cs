using System;
using System.Data;

namespace Toadstool
{
    internal class DbConnectionWrapper : IDbConnectionWrapper, IDbTransactionWrapper
    {
        public CommandBehavior CommandBehavior => _transactionIsComplete ? CommandBehavior.CloseConnection : CommandBehavior.Default;
        internal IDbConnection _dbConnection;
        internal IDbTransaction _dbTransaction;
        private bool _transactionIsComplete;

        public DbConnectionWrapper(IDbConnection dbConnection)
        {
            this._dbConnection = dbConnection;
            _transactionIsComplete = true;
        }

        public DbConnectionWrapper(IDbConnection dbConnection, IDbTransaction transaction)
        {
            this._dbConnection = dbConnection;
            this._dbTransaction = transaction;
        }

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

        public void Dispose(bool force = false)
        {
            if (force || _transactionIsComplete)
            {
                _dbTransaction?.Dispose();
                _dbConnection?.Dispose();
            }
        }
    }
}