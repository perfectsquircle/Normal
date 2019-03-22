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
        public async Task As(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Query("select 7 as alpha, 'foo' as beta, 'something' as charlie, true as delta")
                .ExecuteAsync()
                .AsEnumerableOf<Bar>();

            //Then
            Assert.NotNull(results);
            // Assert.NotEmpty(results); // This iterates the IEnumerable, thus consuming it.
            var bar = results.Single();
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task AsList(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Query("select 7 as alpha, 'foo' as beta, 'something' as charlie, true as delta")
                .ExecuteAsync()
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
                var results = await context
                    .Query("select 1")
                    .ExecuteScalarAsync();

                var results2 = await context
                    .Query("select 2")
                    .ExecuteScalarAsync();

                //Then
                Assert.Equal(1, results as int?);
                Assert.Equal(2, results2 as int?);
                transaction.Commit();
            }

            var results3 = await context
                    .Query("select 1")
                    .ExecuteScalarAsync();
            Assert.Equal(3, results3 as int?);
        }

        public static IEnumerable<object[]> GetDbConnection()
        {
            // yield return new SqlConnection("???"); TODO: support me
            Func<IDbConnection> connectionCreator = () => new NpgsqlConnection("Host=localhost;Database=cfurano;");
            yield return new object[] { connectionCreator };
        }
    }
}