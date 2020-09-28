using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public interface IStatementBuilder : ICommandExecutor
    {
        string Build();
    }
}