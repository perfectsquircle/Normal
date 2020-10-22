using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Normal
{
    internal class MemoryCachingHandler : DelegatingHandler
    {
        public const string NormalTtl = "normal_ttl";
        private readonly IMemoryCache _memoryCache;

        public MemoryCachingHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public override async Task<int> ExecuteNonQueryAsync(ICommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            return await CacheOrNot(commandBuilder, async (buffer) =>
            {
                return await InnerHandler.ExecuteNonQueryAsync(commandBuilder, cancellationToken);
            });
        }

        public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(ICommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            return await CacheOrNot(commandBuilder, async (buffer) =>
            {
                var results = await InnerHandler.ExecuteReaderAsync<T>(commandBuilder, cancellationToken);
                if (buffer)
                {
                    results = results.ToList();
                }
                return results;
            });
        }

        private async Task<T> CacheOrNot<T>(ICommandBuilder commandBuilder, Func<bool, Task<T>> inner)
        {
            if (!commandBuilder.Parameters.ContainsKey(NormalTtl))
            {
                return await inner(false);
            }

            var ttl = GetTtl(commandBuilder);
            var cacheKey = CalculateCacheKey(commandBuilder);

            var results = await _memoryCache.GetOrCreateAsync<T>(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.SlidingExpiration = ttl;
                return await inner(true);
            });
            return results;
        }

        private static TimeSpan? GetTtl(ICommandBuilder command)
        {
            var ttlParam = command.Parameters.First(p => p.Key == NormalTtl);
            var ttl = ttlParam.Value as TimeSpan?;
            command.Parameters.Remove(ttlParam);
            return ttl;
        }

        private static string CalculateCacheKey(ICommandBuilder command)
        {
            using var md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5);
            md5.AppendData(Encoding.UTF8.GetBytes(command.CommandText));
            foreach (var param in command.Parameters)
            {
                md5.AppendData(Encoding.UTF8.GetBytes(param.Key));
                md5.AppendData(Encoding.UTF8.GetBytes(Convert.ToString(param.Value)));
            }
            return Convert.ToBase64String(md5.GetHashAndReset());
        }
    }
}