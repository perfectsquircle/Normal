using System.Data;

namespace Toadstool
{
    public interface IDbContext
    {
        T QueryAsync<T>(object parameters);
        int ExecuteAsync(object parameters);
    }
}