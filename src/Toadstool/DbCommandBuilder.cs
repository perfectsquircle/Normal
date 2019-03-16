using System;
using System.Collections.Generic;
using System.Data;

namespace Toadstool
{
    public class DbCommandBuilder
    {
        string CommandText { get; set; }
        int? CommandTimeout { get; set; }
        CommandType? CommandType { get; set; }
        // IDbConnection Connection { get; set; }
        // IDbTransaction Transaction { get; set; }
        IDictionary<string, string> Parameters { get; }
        public IDbContext DbContext { get; }

        public DbCommandBuilder()
        {
            Parameters = new Dictionary<string, string>();
        }

        internal DbCommandBuilder(IDbContext dbContext) : this()
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

        // public DbCommandBuilder WithConnection(IDbConnection connection)
        // {
        //     this.Connection = connection;
        //     return this;
        // }

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

        // public DbCommandBuilder WithTransaction(IDbTransaction transaction)
        // {
        //     this.Transaction = transaction;
        //     return this;
        // }

        public IDbCommand Build(IDbContext dbContext)
        {
            var dbConnection = dbContext.GetOpenConnection();
            if (dbConnection == null)
            {
                throw new NotSupportedException("Connection required to build command");
            }
            var command = dbConnection.CreateCommand();
            if (CommandText != null) command.CommandText = CommandText;
            if (CommandTimeout != null) command.CommandTimeout = CommandTimeout.Value;
            if (CommandType != null) command.CommandType = CommandType.Value;
            command.Connection = dbConnection;
            command.Transaction = dbContext.GetTransaction();

            foreach (var parameter in Parameters)
            {
                command.Parameters[parameter.Key] = parameter.Value;
            }

            return command;
        }

        public DbResultBuilder Execute()
        {
            return Execute(DbContext);
        }

        internal DbResultBuilder Execute(IDbContext dbContext)
        {
            var command = this.Build(dbContext);
            var reader = command.ExecuteReader();
            return new DbResultBuilder(reader);
        }

        private int ExecuteNonQuery(IDbContext dbContext)
        {
            var command = this.Build(dbContext);
            return command.ExecuteNonQuery();
        }

        private IDataReader ExecuteReader(IDbContext dbContext, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            var command = this.Build(dbContext);
            return command.ExecuteReader(commandBehavior);
        }

        private object ExecuteScalar(IDbContext dbContext)
        {
            var command = this.Build(dbContext);
            return command.ExecuteScalar();
        }
    }
}