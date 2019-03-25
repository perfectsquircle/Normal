using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbCommandBuilder
    {
        public DbCommandBuilder()
        {
            _parameters = new Dictionary<string, string>();
        }

        private string _commandText;
        private int? _commandTimeout;
        private CommandType? _commandType;
        private readonly IDictionary<string, string> _parameters;
        private DbContext _dbContext;

        public DbCommandBuilder WithCommandText(string commandText)
        {
            this._commandText = commandText;
            return this;
        }

        public DbCommandBuilder WithCommandTimeout(int commandTimeout)
        {
            this._commandTimeout = commandTimeout;
            return this;
        }

        public DbCommandBuilder WithCommandType(CommandType commandType)
        {
            this._commandType = commandType;
            return this;
        }

        public DbCommandBuilder WithParameter(string key, object value)
        {
            _parameters[key] = Convert.ToString(value);
            return this;
        }

        public DbCommandBuilder WithParameters(object parameters)
        {
            throw new NotImplementedException();
        }

        public DbCommandBuilder WithParameters(IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<IDbCommand> BuildAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_dbContext == null)
            {
                throw new InvalidOperationException("Context required to build command");
            }
            var dbConnection = await _dbContext.GetOpenConnectionAsync(cancellationToken);
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
            command.Transaction = _dbContext.Transaction;

            foreach (var parameter in _parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }

        public async Task<DbResultBuilder> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var reader = await ExecuteReaderAsync(cancellationToken: cancellationToken);
            return new DbResultBuilder(reader, _dbContext.DataReaderDeserializer);
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildDbCommandAsync(cancellationToken);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IDataReader> ExecuteReaderAsync(CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildDbCommandAsync(cancellationToken);
            return await command.ExecuteReaderAsync(commandBehavior, cancellationToken).ConfigureAwait(false);
        }

        public async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildDbCommandAsync(cancellationToken);
            return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }

        internal DbCommandBuilder WithDbContext(DbContext dbContext)
        {
            _dbContext = dbContext;
            return this;
        }

        private async Task<DbCommand> BuildDbCommandAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await BuildAsync(cancellationToken);
            if (!(command is DbCommand))
            {
                throw new NotSupportedException("Command must be DbCommand");
            }
            return command as DbCommand;
        }
    }
}