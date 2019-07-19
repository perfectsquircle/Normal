using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IStatementBuilder
    {
        string Build();
        Task<int> ExecuteAsync(CancellationToken cancellationToken = default);
        Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default);
    }
}