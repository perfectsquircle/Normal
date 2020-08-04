using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IDbConnectionProvider
    {
        Task<IDbConnectionWrapper> GetOpenConnectionAsync(CancellationToken cancellationToken);
    }
}