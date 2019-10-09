using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IStatementBuilder : IDbCommandExecutor
    {
        string Build();
    }
}