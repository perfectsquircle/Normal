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
        internal DbConnectionWrapper(IDbConnection dbConnection, DbTransactionWrapper transactionContext)
        {
            this.DbConnection = dbConnection;
            this._dbTransactionContext = transactionContext;
        }

        public IDbConnection DbConnection { get; }
        public IDbTransaction DbTransaction => _dbTransactionContext?.DbTransaction;
        public CommandBehavior CommandBehavior => _isComplete ? CommandBehavior.CloseConnection : CommandBehavior.Default;
        private DbTransactionWrapper _dbTransactionContext;
        private bool _isComplete => _dbTransactionContext?.IsComplete ?? true;


        public void Dispose()
        {
            if (_isComplete)
            {
                DbConnection?.Dispose();
            }
        }
    }
}