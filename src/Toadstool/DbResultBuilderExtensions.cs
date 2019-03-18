using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toadstool
{
    public static class DbResultBuilderExtensions
    {
        public static async Task<IList<T>> AsList<T>(this Task<DbResultBuilder> builder)
        {
            return (await builder).As<T>().ToList();
        }

        public static async Task<IEnumerable<T>> As<T>(this Task<DbResultBuilder> builder)
        {
            return (await builder).As<T>();
        }
    }
}