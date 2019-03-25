using System;
using System.Data;

namespace Toadstool
{
    internal class DbConnectionContext : IDbConnectionContext
    {
        internal DbConnectionContext(IDbConnection dbConnection)
        {
            this.DbConnection = dbConnection;
            this.DisposeMe = true;

        }
        internal DbConnectionContext(IDbConnection dbConnection, IDbTransaction transaction)
        {
            this.DbConnection = dbConnection;
            this.DisposeMe = false;

        }

        public IDbConnection DbConnection { get; }
        public IDbTransaction DbTransaction { get; }
        public bool DisposeMe { get; }

        public void Dispose()
        {
            if (DisposeMe)
            {
                DbConnection?.Dispose();
            }
        }
    }
}