using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface IDbContext
    {
        IDbTransaction Transaction { get; }
        IDataReaderDeserializer DataReaderDeserializer { get; }
        DbCommandBuilder Query(string commandText);
        Task<IDbConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}