using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbContext
    {
        DbTransaction Transaction { get; }
        DbCommandBuilder Query(string commandText);
        Task<DbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}