using System;
using System.Collections.Generic;
using System.Data;

namespace Toadstool
{
    internal partial class DbCommandBuilder : IDbCommandBuilder
    {
        public DbCommandBuilder()
        {
            _parameters = new Dictionary<string, object>();
        }

        private string _commandText;
        private int? _commandTimeout;
        private CommandType? _commandType;
        private readonly IDictionary<string, object> _parameters;

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

        internal IDbCommand Build(IDbConnectionWrapper connection)
        {
            var command = connection.CreateCommand();
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

            foreach (var parameter in _parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }
    }
}