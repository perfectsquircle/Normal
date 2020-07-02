using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Normal
{
    public static class IDatabaseBuilderExtensions
    {
        public static IDatabaseBuilder UseCaching(this IDatabaseBuilder builder, IMemoryCache memoryCache)
        {
            return builder.UseDelegatingHandler(new MemoryCachingHandler(memoryCache));
        }

        public static IDatabaseBuilder UseLogging(this IDatabaseBuilder database, ILogger logger)
        {
            return database
                .UseDelegatingHandler(new LoggingHandler(logger));
        }
    }
}