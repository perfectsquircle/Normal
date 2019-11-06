using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal partial class DbCommandBuilder : IDbCommandExecutor
    {
        private IDbConnectionProvider _dbConnectionProvider;
        private readonly IDataRecordMapperFactory _dataRecordMapperFactory = new DataRecordMapperFactory();
        private IHandler _handler;

        public DbCommandBuilder WithDbConnectionProvider(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
            return this;
        }

        internal DbCommandBuilder WithHandler(IHandler handler)
        {
            _handler = handler;
            return this;
        }

        public async Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).ToList(), cancellationToken);
        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).First(), cancellationToken);
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).FirstOrDefault(), cancellationToken);
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).Single(), cancellationToken);
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).SingleOrDefault(), cancellationToken);

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = await _dbConnectionProvider.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connection))
            {
                return await _handler.ExecuteNonQueryAsync(command, cancellationToken);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default)
        {
            using (var connection = await _dbConnectionProvider.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connection))
            {
                return (T)(await _handler.ExecuteScalarAsync(command, cancellationToken));
            }
        }

        private async Task<TReturn> WithReader<TReturn>(Func<IDataReader, TReturn> callback, CancellationToken cancellationToken)
        {
            using (var connection = await _dbConnectionProvider.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connection))
            using (var reader = await _handler.ExecuteReaderAsync(command, cancellationToken))
            {
                return callback.Invoke(reader);
            }
        }

        private DbCommand BuildDbCommand(IDbConnectionWrapper connection)
        {
            var command = Build(connection);
            if (!(command is DbCommand))
            {
                throw new NotSupportedException("Command must be DbCommand");
            }
            return command as DbCommand;
        }

        private IEnumerable<T> ToEnumerable<T>(IDataReader dataReader)
        {
            return ToEnumerable(dataReader, typeof(T)).Cast<T>();
        }

        private IEnumerable ToEnumerable(IDataReader dataReader, Type targetType)
        {
            return ToEnumerable(dataReader, _dataRecordMapperFactory.CreateMapper(targetType));
        }

        private static IEnumerable ToEnumerable(IDataReader dataReader, IDataRecordMapper mapper)
        {
            if (dataReader.FieldCount == 0)
            {
                yield break;
            }

            while (dataReader.Read())
            {
                yield return mapper.MapDataRecord(dataReader);
            }
            yield break;
        }
    }
}