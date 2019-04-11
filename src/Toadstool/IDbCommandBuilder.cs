using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbCommandBuilder
    {
        IDbCommandBuilder WithCommandText(string commandText);
        IDbCommandBuilder WithCommandTimeout(int commandTimeout);
        IDbCommandBuilder WithCommandType(CommandType commandType);
        IDbCommandBuilder WithParameter(string key, object value);
        IDbCommandBuilder WithParameters(object parameters);
        IDbCommandBuilder WithParameters(IDictionary<string, object> parameters);
        Task<IList<T>> AsListOf<T>(CancellationToken cancellationToken = default(CancellationToken));
        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<object> ExecuteScalarAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}