using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
                .Query("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
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
                .ExecuteScalarAsync<int>();

            var results2 = await context
                .Query("select 'hello, there'")
                .ExecuteScalarAsync<string>();

            //Then
            Assert.Equal(1, results);
            Assert.Equal("hello, there", results2);
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
                .ExecuteScalarAsync<int>();

            var results2 = context
                .Query("select 2")
                .ExecuteScalarAsync<int>();

            var results3 = context
                .Query("select 3")
                .ExecuteScalarAsync<int>();

            //Then
            Assert.Equal(1, await results);
            Assert.Equal(2, await results2);
            Assert.Equal(3, await results3);
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
                    .ExecuteScalarAsync<int>();

                var results2 = await context
                    .Query("select 2")
                    .ExecuteScalarAsync<int>();

                var connection2 = context._activeDbConnectionContext.DbConnection;
                var transaction2 = context._activeDbConnectionContext.DbTransaction;

                Assert.Same(connection1, connection2);
                Assert.Same(transaction1, transaction2);

                Assert.Equal(1, results);
                Assert.Equal(2, results2);
                Assert.False(transaction.IsComplete);
                transaction.Commit();
                Assert.True(transaction.IsComplete);
            };
            Assert.Null(context._activeDbConnectionContext);

            var results3 = await context
                    .Query("select 3")
                    .ExecuteScalarAsync<int>();
            Assert.Equal(3, results3);

            using (var transaction = await context.BeginTransactionAsync())
            {
                Assert.NotNull(context._activeDbConnectionContext);

                var results = await context
                    .Query("select 4")
                    .ExecuteScalarAsync<int>();

                //Then
                Assert.Equal(4, results);

                // TRANSACTION NOT COMMITTED
            };
            Assert.Null(context._activeDbConnectionContext);

            var results4 = await context
                    .Query("select 6")
                    .ExecuteScalarAsync<int>();
            Assert.Equal(6, results4);
        }

        public static IEnumerable<object[]> GetDbConnection()
        {
            yield return new object[] { (Func<IDbConnection>)(() => new NpgsqlConnection("Host=localhost;Database=postgres;Username=postgres;Password=toadstool")) };
            yield return new object[] { (Func<IDbConnection>)(() => new SqlConnection("Server=localhost;Uid=sa;Pwd=Toadstool123")) };
        }
    }
}