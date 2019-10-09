using System;
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
        private IDataRecordMapperFactory _dataRecordMapperFactory = new DataRecordMapperFactory();

        public IDbCommandBuilder WithDbConnectionProvider(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
            return this;
        }

        // GENERICS
        public async Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).ToList(), cancellationToken);
        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).First(), cancellationToken, CommandBehavior.SingleRow);
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).FirstOrDefault(), cancellationToken, CommandBehavior.SingleRow);
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).Single(), cancellationToken, CommandBehavior.SingleRow);
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            await WithReader(reader => ToEnumerable<T>(reader).SingleOrDefault(), cancellationToken, CommandBehavior.SingleRow);

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

        private static IEnumerable<T> ToEnumerable<T>(IDataReader dataReader, IDataRecordMapper mapper)
        {
            if (dataReader.FieldCount == 0)
            {
                yield break;
            }

            while (dataReader.Read())
            {
                yield return mapper.MapDataRecord<T>(dataReader);
            }
            yield break;
        }

        private IEnumerable<T> ToEnumerable<T>(IDataReader dataReader)
        {
            return ToEnumerable<T>(dataReader, _dataRecordMapperFactory.CreateMapper(typeof(T)));
        }

        private async Task<TReturn> WithReader<TReturn>(Func<IDataReader, TReturn> callback, CancellationToken cancellationToken, CommandBehavior commandBehavior = default)
        {
            using (var connection = await _dbConnectionProvider.GetOpenConnectionAsync(cancellationToken))
            using (var command = BuildDbCommand(connection))
            using (var reader = await command.ExecuteReaderAsync(commandBehavior, cancellationToken).ConfigureAwait(false))
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