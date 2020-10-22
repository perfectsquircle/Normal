using System;

namespace Normal
{
    public static class ICommandBuilderExtensions
    {
        public static ICommandBuilder CacheFor(this ICommandBuilder builder, TimeSpan cacheTtl)
        {
            return builder.WithParameter(MemoryCachingHandler.NormalTtl, cacheTtl);
        }
    }
}