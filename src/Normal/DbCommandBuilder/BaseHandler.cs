using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Normal
{
    internal class BaseHandler : IHandler
    {
        private DbContext _dbContext;

        public BaseHandler(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            IDbConnectionWrapper connection = null;
            DbCommand command = null;
            DbDataReader reader = null;
            try
            {
                var customMapper = commandBuilder.Mapper;
                connection = await _dbContext.GetOpenConnectionAsync(cancellationToken);
                command = commandBuilder.Build(connection);
                reader = await command.ExecuteReaderAsync(cancellationToken);
                return ToEnumerable<T>(connection, command, reader, customMapper);
            }
            catch
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
                throw;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            using (var connection = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = commandBuilder.Build(connection))
            {
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            using (var connection = await _dbContext.GetOpenConnectionAsync(cancellationToken))
            using (var command = commandBuilder.Build(connection))
            {
                return (T)(await command.ExecuteScalarAsync(cancellationToken));
            }
        }

        private IEnumerable<T> ToEnumerable<T>(IDbConnectionWrapper connection, DbCommand command, DbDataReader dataReader, IDataRecordMapper customMapper = null)
        {
            using (connection)
            using (command)
            using (dataReader)
            {
                if (dataReader.FieldCount == 0)
                {
                    yield break;
                }

                var mapper = customMapper;
                while (dataReader.Read())
                {
                    mapper = mapper ?? _dbContext.DataRecordMapperFactory.CreateMapper(typeof(T));
                    yield return (T)mapper.MapDataRecord(dataReader);
                }
                yield break;
            }
        }
    }
}