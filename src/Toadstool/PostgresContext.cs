using System.Data;
using System.Threading;
using Npgsql;

namespace Toadstool
{
    public class PostgresContext : DbContext
    {
        private readonly string _connectionString;

        public PostgresContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override ThreadLocal<IDbConnection> Connection => new ThreadLocal<IDbConnection>(() => new NpgsqlConnection(_connectionString));
    }
}