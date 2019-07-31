using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal partial class DbCommandBuilder : IDbCommandExecutor
    {
        private IDbConnectionProvider _dbConnectionProvider;
        private IDataRecordMapper _dataRecordMapper = new DefaultDataRecordMapper();

        public IDbCommandBuilder WithDbConnectionProvider(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
            return this;
        }

        // GENERICS
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

        // DYNAMICS
        public async Task<IList<dynamic>> ToListAsync(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable(reader).ToList(), cancellationToken);
        public async Task<dynamic> FirstAsync(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable(reader).First(), cancellationToken);
        public async Task<dynamic> FirstOrDefaultAsync(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable(reader).FirstOrDefault(), cancellationToken);
        public async Task<dynamic> SingleAsync(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable(reader).Single(), cancellationToken);
        public async Task<dynamic> SingleOrDefaultAsync(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable(reader).SingleOrDefault(), cancellationToken);

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = await _dbConnectionProvider.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connection))
            {
                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default)
        {
            using (var connection = await _dbConnectionProvider.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connection))
            {
                return (T)(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
            }
        }

        private static IEnumerable<T> ToEnumerable<T>(IDataReader dataReader, Func<IDataRecord, T> mapper)
        {
            if (dataReader.FieldCount == 0)
            {
                yield break;
            }

            while (dataReader.Read())
            {
                yield return mapper.Invoke(dataReader);
            }
            yield break;
        }

        private IEnumerable<T> ToEnumerable<T>(IDataReader dataReader)
        {
            return ToEnumerable<T>(dataReader, _dataRecordMapper.CompileMapper<T>(dataReader));
        }

        private IEnumerable<dynamic> ToEnumerable(IDataReader dataReader)
        {
            return ToEnumerable(dataReader, _dataRecordMapper.CompileMapper(dataReader));
        }

        private async Task<TReturn> WithReader<TReturn>(Func<IDataReader, TReturn> callback, CancellationToken cancellationToken)
        {
            using (var connection = await _dbConnectionProvider.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connection))
            using (var reader = await command.ExecuteReaderAsync(connection.CommandBehavior, cancellationToken).ConfigureAwait(false))
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
    }
}