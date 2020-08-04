using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal class SingleConnectionDatabase : IDatabase, IDbConnectionProvider
    {
        private IHandler _handler;
        private readonly DbConnection _dbConnection;

        public Variant Variant => Database.DetermineVariant(_dbConnection?.GetType());

        public SingleConnectionDatabase(DbConnection dbConnection)
        {
            _handler = new BaseHandler(this);
            _dbConnection = dbConnection;
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            throw new System.NotImplementedException();
        }

        public IDbCommandBuilder CreateCommand(string commandText)
        {
            return new DbCommandBuilder()
                .WithHandler(_handler)
                .WithCommandText(commandText);
        }

        public void Dispose()
        {
            _dbConnection?.Dispose();
        }

        public async Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                await _dbConnection.OpenAsync();
            }
            return new DbConnectionWrapper(_dbConnection) { DisposeDisabled = true };
        }
    }
}