using System;
using System.Data;

namespace Toadstool
{
    internal class DbConnectionContext : IDbConnectionContext
    {
        internal DbConnectionContext(IDbConnection dbConnection)
        {
            this.DbConnection = dbConnection;

        }
        internal DbConnectionContext(IDbConnection dbConnection, DbTransactionContext transactionContext)
        {
            this.DbConnection = dbConnection;
            this._dbTransactionContext = transactionContext;

        }

        public IDbConnection DbConnection { get; }
        public IDbTransaction DbTransaction => _dbTransactionContext?.DbTransaction;
        public bool IsComplete => _dbTransactionContext?.IsComplete ?? true;
        public CommandBehavior CommandBehavior => IsComplete ? CommandBehavior.CloseConnection : CommandBehavior.Default;
        private DbTransactionContext _dbTransactionContext;


        public void Dispose()
        {
            if (IsComplete)
            {
                DbConnection?.Dispose();
            }
        }
    }
}