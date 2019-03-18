using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbCommandBuilder
    {
        string CommandText { get; set; }
        int? CommandTimeout { get; set; }
        CommandType? CommandType { get; set; }
        IDictionary<string, string> Parameters { get; }
        public DbContext DbContext { get; }

        public DbCommandBuilder()
        {
            Parameters = new Dictionary<string, string>();
        }

        internal DbCommandBuilder(DbContext dbContext) : this()
        {
            DbContext = dbContext;
        }

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

        public async Task<DbCommand> BuildAsync(IDbContext dbContext)
        {
            var dbConnection = await dbContext.GetOpenConnectionAsync();
            if (dbConnection == null)
            {
                throw new NotSupportedException("Connection required to build command");
            }
            var command = dbConnection.CreateCommand();
            if (CommandText != null) command.CommandText = CommandText;
            if (CommandTimeout != null) command.CommandTimeout = CommandTimeout.Value;
            if (CommandType != null) command.CommandType = CommandType.Value;
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

        public Task<DbResultBuilder> ExecuteAsync()
        {
            return ExecuteAsync(DbContext);
        }

        internal async Task<DbResultBuilder> ExecuteAsync(IDbContext dbContext)
        {
            var command = await this.BuildAsync(dbContext);
            var reader = await command.ExecuteReaderAsync();
            return new DbResultBuilder(reader);
        }

        public async Task<int> ExecuteNonQueryAsync(IDbContext dbContext)
        {
            var command = await this.BuildAsync(dbContext);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<IDataReader> ExecuteReaderAsync(IDbContext dbContext, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            var command = await this.BuildAsync(dbContext);
            return await command.ExecuteReaderAsync(commandBehavior);
        }

        public async Task<object> ExecuteScalarAsync(IDbContext dbContext)
        {
            var command = await this.BuildAsync(dbContext);
            return command.ExecuteScalarAsync();
        }
    }
}