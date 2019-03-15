using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbContextProvider
    {
        Task<IDbContext> GetDbContextAsync();
        Task<IDbContext> GetDbContextAsync(CancellationToken cancellationToken);
    }
}