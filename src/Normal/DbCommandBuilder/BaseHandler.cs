using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal class BaseHandler : IHandler
    {
        private readonly IDbConnectionProvider _database;
        private readonly IDataRecordMapperFactory _dataRecordMapperFactory;

        public BaseHandler(IDbConnectionProvider database)
            : this(database, new DataRecordMapperFactory())
        {
        }

        public BaseHandler(IDbConnectionProvider database, IDataRecordMapperFactory dataRecordMapperFactory)
        {
            _database = database;
            _dataRecordMapperFactory = dataRecordMapperFactory;
        }

        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            IDbConnectionWrapper connection = null;
            DbCommand command = null;
            DbDataReader reader = null;
            try
            {
                connection = await _database.GetOpenConnectionAsync(cancellationToken);
                command = (commandBuilder as DbCommandBuilder).Build(connection);
                reader = await command.ExecuteReaderAsync(cancellationToken);
                return ToEnumerable<T>(connection, command, reader);
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
            using (var connection = await _database.GetOpenConnectionAsync(cancellationToken))
            using (var command = (commandBuilder as DbCommandBuilder).Build(connection))
            {
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(IDbCommandBuilder commandBuilder, CancellationToken cancellationToken)
        {
            using (var connection = await _database.GetOpenConnectionAsync(cancellationToken))
            using (var command = (commandBuilder as DbCommandBuilder).Build(connection))
            {
                return (T)await command.ExecuteScalarAsync(cancellationToken);
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

                IDataRecordMapper<T> mapper = null;
                while (dataReader.Read())
                {
                    mapper = mapper ?? _dataRecordMapperFactory.CreateMapper<T>();
                    yield return mapper.MapDataRecord(dataReader);
                }
                yield break;
            }
        }
    }
}