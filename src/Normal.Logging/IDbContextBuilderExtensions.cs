using Microsoft.Extensions.Logging;

namespace Normal
{
    public static class IDbContextBuilderExtensions
    {
        public static IDbContextBuilder UseLogging(this IDbContextBuilder dbContext, ILogger logger)
        {
            return dbContext
                .UseDelegatingHandler(new LoggingHandler(logger));
        }
    }
}