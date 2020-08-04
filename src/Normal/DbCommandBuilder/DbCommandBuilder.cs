using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Normal
{
    internal partial class DbCommandBuilder : IDbCommandBuilder
    {
        public string CommandText { get; private set; }
        public int? CommandTimeout { get; private set; }
        public CommandType? CommandType { get; private set; }
        public IDictionary<string, object> Parameters { get; }

        public DbCommandBuilder()
        {
            Parameters = new Dictionary<string, object>();
        }

        public IDbCommandBuilder WithCommandText(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            CommandText = commandText;
            return this;
        }

        public IDbCommandBuilder WithCommandTimeout(int commandTimeout)
        {
            CommandTimeout = commandTimeout;
            return this;
        }

        public IDbCommandBuilder WithCommandType(CommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        public IDbCommandBuilder WithParameter(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            Parameters[key] = value;
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
                Parameters[parameter.Key] = parameter.Value;
            }
            return this;
        }

        public DbCommand Build(IDbConnectionWrapper connection)
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
                if (!(parameter.Value is string) && parameter.Value is IEnumerable list)
                {
                    AddListParameters(command, parameter.Key, list);
                    continue;
                }

                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value ?? DBNull.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }

        private static void AddListParameters(DbCommand command, string prefix, IEnumerable list)
        {
            var i = 1;
            var newParameterNames = new List<string>();
            foreach (var value in list)
            {
                var listParameter = command.CreateParameter();
                newParameterNames.Add(
                    listParameter.ParameterName = $"{prefix}_{i++}");
                listParameter.Value = value ?? DBNull.Value;
                command.Parameters.Add(listParameter);
            }
            command.CommandText = InjectListParameters(command.CommandText, prefix, newParameterNames);
        }

        private static string InjectListParameters(string commandText, string key, List<string> newParameterNames)
        {
            return commandText.Replace(
                "@" + key.TrimStart('@'),
                string.Join(", ", newParameterNames.Select(p => "@" + p))
            );
        }
    }
}