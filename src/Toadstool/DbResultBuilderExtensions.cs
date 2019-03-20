using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toadstool
{
    public static class DbResultBuilderExtensions
    {
        public static async Task<IList<T>> AsListOf<T>(this Task<DbResultBuilder> builder)
        {
            return (await builder).AsEnumerableOf<T>().ToList();
        }

        public static async Task<IEnumerable<T>> AsEnumerableOf<T>(this Task<DbResultBuilder> builder)
        {
            return (await builder).AsEnumerableOf<T>();
        }
    }
}