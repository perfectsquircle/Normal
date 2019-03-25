using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbContext
    {
        DbCommandBuilder Query(string commandText);
        Task BeginTransactionAsync(Func<IDbTransaction, Task> transactionAction, CancellationToken cancellationToken = default(CancellationToken));
    }
}