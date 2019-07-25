using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IStatementBuilder : IDbCommandExecutor
    {
        string Build();
    }
}