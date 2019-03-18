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

        internal DbCommandBuilder(DbContext dbContext)
            : this()
        {
            DbContext = dbContext;
        }

        public string CommandText { get; set; }
        public int? CommandTimeout { get; set; }
        public CommandType? CommandType { get; set; }
        public IDictionary<string, string> Parameters { get; }
        public DbContext DbContext { get; }

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

        public async Task<DbCommand> BuildAsync(IDbContext dbContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dbConnection = await dbContext.GetOpenConnectionAsync(cancellationToken);
            if (dbConnection == null)
            {
                throw new NotSupportedException("Connection required to build command");
            }
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
            command.Transaction = dbContext.Transaction;

            foreach (var parameter in Parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }

        public Task<DbResultBuilder> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(DbContext, cancellationToken);
        }

        public async Task<int> ExecuteNonQueryAsync(IDbContext dbContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildAsync(dbContext, cancellationToken);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IDataReader> ExecuteReaderAsync(IDbContext dbContext, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildAsync(dbContext, cancellationToken);
            return await command.ExecuteReaderAsync(commandBehavior, cancellationToken).ConfigureAwait(false);
        }

        public async Task<object> ExecuteScalarAsync(IDbContext dbContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await this.BuildAsync(dbContext, cancellationToken);
            return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        }

        // internal
        internal async Task<DbResultBuilder> ExecuteAsync(IDbContext dbContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var reader = await ExecuteReaderAsync(dbContext, cancellationToken: cancellationToken);
            return new DbResultBuilder(reader, dbContext.DataReaderDeserializer);
        }
    }
}