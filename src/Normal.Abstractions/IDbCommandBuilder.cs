using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Normal
{
    public interface IDbCommandBuilder : IDbCommandExecutor
    {
        string CommandText { get; }
        IDictionary<string, object> Parameters { get; }
        int? CommandTimeout { get; }
        CommandType? CommandType { get; }
        IDataRecordMapper Mapper { get; }
        IDbCommandBuilder WithCommandText(string commandText);
        IDbCommandBuilder WithCommandTimeout(int commandTimeout);
        IDbCommandBuilder WithCommandType(CommandType commandType);
        IDbCommandBuilder WithParameter(string key, object value);
        IDbCommandBuilder WithParameters(object parameters);
        IDbCommandBuilder WithParameters(IDictionary<string, object> parameters);
        DbCommand Build(IDbConnectionWrapper connection);
    }
}