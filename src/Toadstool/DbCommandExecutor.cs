using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal partial class DbCommandBuilder : IDbCommandExecutor
    {
        private DbContext _dbContext;

        internal IDbCommandBuilder WithDbContext(DbContext dbContext)
        {
            _dbContext = dbContext;
            return this;
        }

        public async Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connectionContext = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connectionContext))
            using (var reader = await command.ExecuteReaderAsync(connectionContext.CommandBehavior, cancellationToken).ConfigureAwait(false))
            {
                return ToEnumerableAsync<T>(reader).ToList();
            }
        }

        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) => (await ToListAsync<T>(cancellationToken)).First();
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) => (await ToListAsync<T>(cancellationToken)).FirstOrDefault();
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) => (await ToListAsync<T>(cancellationToken)).Single();
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) => (await ToListAsync<T>(cancellationToken)).SingleOrDefault();

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connectionContext = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connectionContext))
            {
                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connectionContext = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connectionContext))
            {
                return (T)(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
            }
        }

        internal IDbCommand Build(IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            var command = dbConnection.CreateCommand();
            if (_commandText != null)
            {
                command.CommandText = _commandText;
            }
            if (_commandTimeout != null)
            {
                command.CommandTimeout = _commandTimeout.Value;
            }
            if (_commandType != null)
            {
                command.CommandType = _commandType.Value;
            }
            command.Connection = dbConnection;
            command.Transaction = dbTransaction;

            foreach (var parameter in _parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }

        private DbCommand BuildDbCommand(IDbConnectionWrapper dbConnectionContext)
        {
            var command = Build(dbConnectionContext.DbConnection, dbConnectionContext.DbTransaction);
            if (!(command is DbCommand))
            {
                throw new NotSupportedException("Command must be DbCommand");
            }
            return command as DbCommand;
        }

        private IEnumerable<T> ToEnumerableAsync<T>(IDataReader dataReader)
        {
            if (dataReader.FieldCount == 0)
            {
                yield break;
            }

            Func<IDataRecord, T> mapper = null;

            while (dataReader.Read())
            {
                if (mapper == null)
                {
                    mapper = _dbContext.DataRecordMapper.CompileMapper<T>(dataReader);
                }
                yield return mapper.Invoke(dataReader);
            }
            yield break;
        }
    }
}