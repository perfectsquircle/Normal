using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal class BaseHandler : IHandler
    {
        private DbContext _dbContext;
        private readonly IDataRecordMapperFactory _dataRecordMapperFactory = new DataRecordMapperFactory();

        public BaseHandler(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            var connection = await _dbContext.GetOpenConnectionAsync(cancellationToken);
            var command = commandBuilder.Build(connection);
            var reader = await command.ExecuteReaderAsync(cancellationToken);
            return ToEnumerable<T>(connection, command, reader);
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

        private IEnumerable<T> ToEnumerable<T>(IDbConnectionWrapper connection, DbCommand command, DbDataReader dataReader)
        {
            using (connection)
            using (command)
            using (dataReader)
            {
                if (dataReader.FieldCount == 0)
                {
                    yield break;
                }

                IDataRecordMapper mapper = null;
                while (dataReader.Read())
                {
                    mapper = mapper ?? _dataRecordMapperFactory.CreateMapper(typeof(T));
                    yield return (T)mapper.MapDataRecord(dataReader);
                }
                yield break;
            }
        }
    }
}