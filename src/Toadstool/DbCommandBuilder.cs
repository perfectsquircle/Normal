using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal class DbCommandBuilder : IDbCommandBuilder
    {
        public DbCommandBuilder()
        {
            _parameters = new Dictionary<string, object>();
        }

        private string _commandText;
        private int? _commandTimeout;
        private CommandType? _commandType;
        private readonly IDictionary<string, object> _parameters;
        private DbContext _dbContext;

        public IDbCommandBuilder WithDbContext(DbContext dbContext)
        {
            _dbContext = dbContext;
            return this;
        }

        public IDbCommandBuilder WithCommandText(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            this._commandText = commandText;
            return this;
        }

        public IDbCommandBuilder WithCommandTimeout(int commandTimeout)
        {
            this._commandTimeout = commandTimeout;
            return this;
        }

        public IDbCommandBuilder WithCommandType(CommandType commandType)
        {
            this._commandType = commandType;
            return this;
        }

        public IDbCommandBuilder WithParameter(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _parameters[key] = value;
            return this;
        }

        public IDbCommandBuilder WithParameters(object parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return WithParameters(ReflectionHelper.ToDictionary(parameters));
        }

        public IDbCommandBuilder WithParameters(IDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            foreach (var parameter in parameters)
            {
                if (parameter.Key == null)
                {
                    continue;
                }
                _parameters[parameter.Key] = parameter.Value;
            }
            return this;
        }

        public async Task<IList<T>> AsListOf<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connectionContext = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connectionContext))
            using (var reader = await command.ExecuteReaderAsync(connectionContext.CommandBehavior, cancellationToken).ConfigureAwait(false))
            {
                return AsEnumerableOf<T>(reader).ToList();
            }
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connectionContext = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connectionContext))
            {
                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connectionContext = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connectionContext))
            {
                return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
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
                dbParameter.Value = parameter.Value;
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

        private IEnumerable<T> AsEnumerableOf<T>(IDataReader dataReader)
        {
            if (dataReader.FieldCount == 0)
            {
                yield break;
            }

            while (dataReader.Read())
            {
                yield return _dbContext.DataRecordDeserializer.Deserialize<T>(dataReader);
            }
            yield break;
        }
    }
}