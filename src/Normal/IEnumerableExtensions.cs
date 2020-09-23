using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Normal
{
    public static class IEnumerableExtensions
    {
        public static async Task<T> FirstAsync<T>(this Task<IEnumerable<T>> list)
            => (await list).First();
        public static async Task<T> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> list)
            => (await list).FirstOrDefault();
        public static async Task<T> SingleAsync<T>(this Task<IEnumerable<T>> list)
            => (await list).Single();
        public static async Task<T> SingleOrDefaultAsync<T>(this Task<IEnumerable<T>> list)
            => (await list).SingleOrDefault();
    }
}