using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal partial class CommandBuilder : ICommandBuilder
    {
        public string CommandText { get; private set; }
        public int? CommandTimeout { get; private set; }
        public CommandType? CommandType { get; private set; }
        public IDictionary<string, object> Parameters { get; }

        public CommandBuilder()
        {
            Parameters = new Dictionary<string, object>();
        }

        public ICommandBuilder WithCommandText(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            CommandText = commandText;
            return this;
        }

        public ICommandBuilder WithCommandTimeout(int commandTimeout)
        {
            CommandTimeout = commandTimeout;
            return this;
        }

        public ICommandBuilder WithCommandType(CommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        public ICommandBuilder WithParameter(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            Parameters[key] = value;
            return this;
        }

        public ICommandBuilder WithParameters(object parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return WithParameters(ReflectionHelper.ToDictionary(parameters));
        }

        public ICommandBuilder WithParameters(IDictionary<string, object> parameters)
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
                Parameters[parameter.Key] = parameter.Value;
            }
            return this;
        }

        public DbCommand Build(IConnection connection)
        {
            var command = connection.CreateCommand();
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

            foreach (var parameter in Parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }
    }

    internal partial class CommandBuilder : ICommandExecutor
    {
        private IHandler _handler;

        internal CommandBuilder WithHandler(IHandler handler)
        {
            _handler = handler;
            return this;
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken));
        public async Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).ToList();
        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).First();
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).FirstOrDefault();
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).Single();
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            (await ToEnumerable<T>(cancellationToken)).SingleOrDefault();

        private async Task<IEnumerable<T>> ToEnumerable<T>(CancellationToken cancellationToken)
        {
            return await _handler.ExecuteReaderAsync<T>(this, cancellationToken);
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
        {
            return await _handler.ExecuteNonQueryAsync(this, cancellationToken);
        }
    }
}