using Microsoft.Extensions.Logging;

namespace Normal
{
    public static class IDatabaseBuilderExtensions
    {
        public static IDatabaseBuilder UseLogging(this IDatabaseBuilder database, ILogger logger)
        {
            return database
                .UseDelegatingHandler(new LoggingHandler(logger));
        }
    }
}