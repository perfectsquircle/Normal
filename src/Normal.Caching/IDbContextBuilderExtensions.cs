using Microsoft.Extensions.Caching.Memory;

namespace Normal
{
    public static class IDbContextBuilderExtensions
    {
        public static IDbContextBuilder UseCaching(this IDbContextBuilder dbContext, IMemoryCache memoryCache)
        {
            return dbContext
                .UseDelegatingHandler(new CachingHandler(memoryCache));
        }
    }
}