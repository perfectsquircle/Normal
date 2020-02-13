using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal class BaseHandler : IHandler
    {
        private readonly Database _database;
        private readonly IDataRecordMapperFactory _dataRecordMapperFactory;

        public BaseHandler(Database database)
            : this(database, new DataRecordMapperFactory())
        {
        }

        public BaseHandler(Database database, IDataRecordMapperFactory dataRecordMapperFactory)
        {
            this._database = database;
            this._dataRecordMapperFactory = dataRecordMapperFactory;
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
                return ToEnumerable<T>(connection, command, reader, commandBuilder.Mapper);
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
                    mapper = mapper ?? _dataRecordMapperFactory.CreateMapper(typeof(T));
                    yield return (T)mapper.MapDataRecord(dataReader);
                }
                yield break;
            }
        }
    }
}