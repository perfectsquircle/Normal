using Microsoft.Extensions.Caching.Memory;

namespace Normal
{
    public static class IDbContextBuilderExtensions
    {
        public static IDbContextBuilder UseCaching(this IDbContextBuilder builder, IMemoryCache memoryCache)
        {
            return builder.UseDelegatingHandler(new MemoryCachingHandler(memoryCache));
        }
    }
}