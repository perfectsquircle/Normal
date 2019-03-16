using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Npgsql;

namespace Toadstool
{
    public class SqlServerContext : DbContext
    {
        private readonly string _connectionString;

        public SqlServerContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override ThreadLocal<IDbConnection> Connection => new ThreadLocal<IDbConnection>(() => new SqlConnection(_connectionString));
    }
}