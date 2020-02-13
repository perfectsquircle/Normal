using Microsoft.Extensions.Caching.Memory;

namespace Normal
{
    public static class IDatabaseBuilderExtensions
    {
        public static IDatabaseBuilder UseCaching(this IDatabaseBuilder builder, IMemoryCache memoryCache)
        {
            return builder.UseDelegatingHandler(new MemoryCachingHandler(memoryCache));
        }
    }
}