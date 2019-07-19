using System.Collections.Generic;
using System.Data;

namespace Toadstool
{
    public interface IDbCommandBuilder : IDbCommandExecutor
    {
        IDbCommandBuilder WithCommandText(string commandText);
        IDbCommandBuilder WithCommandTimeout(int commandTimeout);
        IDbCommandBuilder WithCommandType(CommandType commandType);
        IDbCommandBuilder WithParameter(string key, object value);
        IDbCommandBuilder WithParameters(object parameters);
        IDbCommandBuilder WithParameters(IDictionary<string, object> parameters);
    }
}