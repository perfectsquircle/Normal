using System;

namespace Normal
{
    public static class IDbCommandBuilderExtensions
    {
        public static IDbCommandBuilder CacheFor(this IDbCommandBuilder builder, TimeSpan cacheTtl)
        {
            return builder.WithParameter(CachingHandler.NormalTtl, cacheTtl);
        }
    }
}