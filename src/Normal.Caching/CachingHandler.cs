using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
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

        public override Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return InnerHandler.ExecuteNonQueryAsync(command, cancellationToken);
        }

        public override async Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (!command.Parameters.Contains(NormalTtl))
            {
                return await InnerHandler.ExecuteReaderAsync(command, cancellationToken);
            }

            var cacheKey = CalculateCacheKey(command);
            var ttl = GetTtl(command);

            var dataTable = await memoryCache.GetOrCreateAsync<DataTable>(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.SlidingExpiration = ttl;
                using (var reader = await InnerHandler.ExecuteReaderAsync(command, cancellationToken))
                {
                    DataTable t = new DataTable();
                    t.Load(reader);
                    return t;
                }
            });
            return dataTable.CreateDataReader();
        }

        private static TimeSpan? GetTtl(DbCommand command)
        {
            var ttlParam = command.Parameters[NormalTtl];
            var ttl = ttlParam.Value as TimeSpan?;
            command.Parameters.Remove(ttlParam);
            return ttl;
        }

        private static string CalculateCacheKey(DbCommand command)
        {
            string hash;
            using (var md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            {
                // For each block:
                md5.AppendData(Encoding.UTF8.GetBytes(command.CommandText));
                foreach (var param in command.Parameters.Cast<DbParameter>())
                {
                    md5.AppendData(Encoding.UTF8.GetBytes(param.ParameterName));
                    md5.AppendData(Encoding.UTF8.GetBytes(Convert.ToString(param.Value)));
                }
                hash = Convert.ToBase64String(md5.GetHashAndReset());
            }

            return hash;
        }

        public override async Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return await InnerHandler.ExecuteScalarAsync(command, cancellationToken);
        }
    }
}