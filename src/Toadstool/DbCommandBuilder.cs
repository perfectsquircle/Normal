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
            Parameters = new Dictionary<string, string>();
        }

        public string CommandText { get; private set; }
        public int? CommandTimeout { get; private set; }
        public CommandType? CommandType { get; private set; }
        public IDictionary<string, string> Parameters { get; }
        public DbContext DbContext { get; private set; }

        public DbCommandBuilder WithCommandText(string commandText)
        {
            this.CommandText = commandText;
            return this;
        }

        public DbCommandBuilder WithCommandTimeout(int commandTimeout)
        {
            this.CommandTimeout = commandTimeout;
            return this;
        }

        public DbCommandBuilder WithCommandType(CommandType commandType)
        {
            this.CommandType = commandType;
            return this;
        }

        public DbCommandBuilder WithParameter(string key, object value)
        {
            Parameters[key] = Convert.ToString(value);
            return this;
        }

        public DbCommandBuilder WithParameters(object parameters)
        {
            throw new NotImplementedException();
        }

        public DbCommandBuilder WithParameters(IDictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public DbCommandBuilder WithDbContext(DbContext dbContext)
        {
            DbContext = dbContext;
            return this;
        }

        public async Task<DbCommand> BuildAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DbContext == null)
            {
                throw new InvalidOperationException("Context required to build command");
            }
            var dbConnection = await DbContext.GetOpenConnectionAsync(cancellationToken);
            var command = dbConnection.CreateCommand();
            if (CommandText != null)
            {
                command.CommandText = CommandText;
            }
            if (CommandTimeout != null)
            {
                command.CommandTimeout = CommandTimeout.Value;
            }
            if (CommandType != null)
            {
                command.CommandType = CommandType.Value;
            }
            command.Connection = dbConnection;
            command.Transaction = DbContext.Transaction;

            foreach (var parameter in Parameters)
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
            return new DbResultBuilder(reader, DbContext.DataReaderDeserializer);
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildAsync(cancellationToken);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IDataReader> ExecuteReaderAsync(CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildAsync(cancellationToken);
            return await command.ExecuteReaderAsync(commandBehavior, cancellationToken).ConfigureAwait(false);
        }

        public async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildAsync(cancellationToken);
            return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}