using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Normal
{
    public interface ICommandBuilder : ICommandExecutor
    {
        string CommandText { get; }
        IDictionary<string, object> Parameters { get; }
        int? CommandTimeout { get; }
        CommandType? CommandType { get; }
        ICommandBuilder WithCommandText(string commandText);
        ICommandBuilder WithCommandTimeout(int commandTimeout);
        ICommandBuilder WithCommandType(CommandType commandType);
        ICommandBuilder WithParameter(string key, object value);
        ICommandBuilder WithParameters(object parameters);
        ICommandBuilder WithParameters(IDictionary<string, object> parameters);
        DbCommand Build(IConnection connection);
    }
}