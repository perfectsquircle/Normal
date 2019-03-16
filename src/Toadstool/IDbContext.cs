using System.Data;

namespace Toadstool
{
    public interface IDbContext
    {
        DbCommandBuilder Query(string commandText);

        IDbConnection GetOpenConnection();
        IDbTransaction GetTransaction();
    }
}