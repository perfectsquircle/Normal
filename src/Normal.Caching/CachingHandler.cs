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
    public class CachingHandler : DelegatingHandler
    {
        public const string NormalTtl = "normal_ttl";
        private IMemoryCache memoryCache;

        public CachingHandler(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public override Task<int> ExecuteNonQueryAsync(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            return InnerHandler.ExecuteNonQueryAsync(commandBuilder, cancellationToken);
        }

        public override async Task<T> ExecuteScalarAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            return await InnerHandler.ExecuteScalarAsync<T>(commandBuilder, cancellationToken);
        }

        public override async Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            if (!commandBuilder.Parameters.ContainsKey(NormalTtl))
            {
                return await InnerHandler.ExecuteReaderAsync<T>(commandBuilder, cancellationToken);
            }

            var cacheKey = CalculateCacheKey(commandBuilder);
            var ttl = GetTtl(commandBuilder);

            var results = await memoryCache.GetOrCreateAsync<IEnumerable<T>>(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.SlidingExpiration = ttl;
                var results = await InnerHandler.ExecuteReaderAsync<T>(commandBuilder, cancellationToken);
                return results.ToList();
            });
            return results;
        }

        private static TimeSpan? GetTtl(IDbCommandBuilder command)
        {
            var ttlParam = command.Parameters.First(p => p.Key == NormalTtl);
            var ttl = ttlParam.Value as TimeSpan?;
            command.Parameters.Remove(ttlParam);
            return ttl;
        }

        private static string CalculateCacheKey(IDbCommandBuilder command)
        {
            using (var md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            {
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
}