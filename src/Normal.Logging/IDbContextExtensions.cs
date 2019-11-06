using Microsoft.Extensions.Logging;

namespace Normal
{
    public static class IDbContextExtensions
    {
        public static IDbContextBuilder WithLogging(this IDbContextBuilder dbContext, ILogger logger)
        {
            return dbContext
                .WithDelegatingHandler(new LoggingHandler(logger));
        }
    }
}