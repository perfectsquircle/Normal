using System;
using System.Collections.Generic;
using System.Data;

namespace Normal
{
    internal partial class DbCommandBuilder : IDbCommandBuilder
    {
        private string _commandText;
        private int? _commandTimeout;
        private CommandType? _commandType;
        private IDataRecordMapper _mapper;
        private readonly IDictionary<string, object> _parameters;

        public DbCommandBuilder()
        {
            _parameters = new Dictionary<string, object>();
        }

        public IDbCommandBuilder WithCommandText(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            _commandText = commandText;
            return this;
        }

        public IDbCommandBuilder WithCommandTimeout(int commandTimeout)
        {
            _commandTimeout = commandTimeout;
            return this;
        }

        public IDbCommandBuilder WithCommandType(CommandType commandType)
        {
            _commandType = commandType;
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

        public IDbCommandBuilder WithDataRecordMapper(IDataRecordMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            _mapper = mapper;
            return this;
        }

        public IDbCommandBuilder WithDataRecordMapper(Type type, MapDataRecord mapDataRecord)
        {
            if (mapDataRecord == null)
            {
                throw new ArgumentNullException(nameof(mapDataRecord));
            }
            _mapper = new AdHocDataRecordMapper(mapDataRecord);
            return this;
        }

        public IDbCommand Build(IDbConnectionWrapper connection)
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