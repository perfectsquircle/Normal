using System;
using System.Data;

namespace Toadstool
{
    internal class DbConnectionWrapper : IDbConnectionWrapper
    {
        internal DbConnectionWrapper(IDbConnection dbConnection)
        {
            this.DbConnection = dbConnection;
        }
        internal DbConnectionWrapper(IDbConnection dbConnection, DbTransactionWrapper transactionWrapper)
        {
            this.DbConnection = dbConnection;
            this._dbTransactionWrapper = transactionWrapper;
        }

        public IDbConnection DbConnection { get; }
        public IDbTransaction DbTransaction => _dbTransactionWrapper?.DbTransaction;
        public CommandBehavior CommandBehavior => _isComplete ? CommandBehavior.CloseConnection : CommandBehavior.Default;
        private DbTransactionWrapper _dbTransactionWrapper;
        private bool _isComplete => _dbTransactionWrapper?.IsComplete ?? true;


        public void Dispose()
        {
            if (_isComplete)
            {
                DbConnection?.Dispose();
            }
        }
    }
}