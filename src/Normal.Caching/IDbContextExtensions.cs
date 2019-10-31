
using Microsoft.Extensions.Caching.Memory;

namespace Normal
{
    public static class IDbContextExtensions
    {
        public static IDbContextBuilder WithCaching(this IDbContextBuilder dbContext, IMemoryCache memoryCache)
        {
            return dbContext
                .WithDelegatingHandler(new CachingHandler(memoryCache));
        }
    }
}