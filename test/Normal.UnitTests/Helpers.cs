using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Normal.UnitTests
{
    public static class Helpers
    {
        public static Microsoft.Extensions.Logging.ILogger GetLogger()
        {
            var serilogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var factory = new LoggerFactory().AddSerilog(serilogger);
            return factory.CreateLogger(nameof(WideWorldImportersTests));
        }

        internal static Microsoft.Extensions.Caching.Memory.IMemoryCache GetMemoryCache()
        {
            return new MemoryCache(new MemoryCacheOptions());
        }
    }
}