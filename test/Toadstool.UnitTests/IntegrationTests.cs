using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Toadstool.UnitTests.Fixtures;
using Xunit;

namespace Toadstool.UnitTests
{
    public class IntegrationTests
    {
        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task AsListOf(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Query("select 7 as alpha, 'foo' as beta, 'something' as charlie, true as delta")
                .AsListOf<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            var bar = results.Single();
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task MultipleQueries(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Query("select 1")
                .ExecuteScalarAsync();

            var results2 = await context
                .Query("select 2")
                .ExecuteScalarAsync();

            //Then
            Assert.Equal(1, results as int?);
            Assert.Equal(2, results2 as int?);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SimultaneousQueries(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = context
                .Query("select 1")
                .ExecuteScalarAsync();

            var results2 = context
                .Query("select 2")
                .ExecuteScalarAsync();

            var results3 = context
                .Query("select 3")
                .ExecuteScalarAsync();

            //Then
            Assert.Equal(1, await results as int?);
            Assert.Equal(2, await results2 as int?);
            Assert.Equal(3, await results3 as int?);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task MultipleQueriesInTransaction(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            using (var transaction = await context.BeginTransactionAsync())
            {
                Assert.NotNull(context._activeDbConnectionContext);
                var connection1 = context._activeDbConnectionContext.DbConnection;
                var transaction1 = context._activeDbConnectionContext.DbTransaction;

                var results = await context
                    .Query("select 1")
                    .ExecuteScalarAsync();

                var results2 = await context
                    .Query("select 2")
                    .ExecuteScalarAsync();

                var connection2 = context._activeDbConnectionContext.DbConnection;
                var transaction2 = context._activeDbConnectionContext.DbTransaction;

                Assert.Same(connection1, connection2);
                Assert.Same(transaction1, transaction2);

                Assert.Equal(1, results as int?);
                Assert.Equal(2, results2 as int?);
                Assert.False(transaction.IsComplete);
                transaction.Commit();
                Assert.True(transaction.IsComplete);
            };
            Assert.Null(context._activeDbConnectionContext);

            var results3 = await context
                    .Query("select 3")
                    .ExecuteScalarAsync();
            Assert.Equal(3, results3 as int?);

            using (var transaction = await context.BeginTransactionAsync())
            {
                Assert.NotNull(context._activeDbConnectionContext);

                var results = await context
                    .Query("select 4")
                    .ExecuteScalarAsync();

                //Then
                Assert.Equal(4, results as int?);

                // TRANSACTION NOT COMMITTED
            };
            Assert.Null(context._activeDbConnectionContext);

            var results4 = await context
                    .Query("select 6")
                    .ExecuteScalarAsync();
            Assert.Equal(6, results4 as int?);
        }

        public static IEnumerable<object[]> GetDbConnection()
        {
            // yield return new SqlConnection("???"); TODO: support me
            Func<IDbConnection> connectionCreator = () => new NpgsqlConnection("Host=postgres;Database=postgres;Username=postgres;Password=toadstool");
            yield return new object[] { connectionCreator };
        }
    }
}