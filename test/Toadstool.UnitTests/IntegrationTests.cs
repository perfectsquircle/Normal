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
        public async Task ToListAsync(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            IList<Bar> results = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .ToListAsync<Bar>();

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
        public async Task ToListAsyncIsEmpty(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            IList<Bar> results = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                .ToListAsync<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstAsync(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var bar = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .FirstAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstAsyncThrows(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            await Assert.ThrowsAsync<System.InvalidOperationException>(async () =>
            {
                var bar = await context
                    .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                    .FirstAsync<Bar>();
            });
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstOrDefaultAsync(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var bar = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .FirstOrDefaultAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstOrDefaultAsyncIsNull(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var bar = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                .FirstOrDefaultAsync<Bar>();

            //Then
            Assert.Null(bar);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleAsync(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var bar = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .SingleAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleAsyncThrows(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            await Assert.ThrowsAsync<System.InvalidOperationException>(async () =>
            {
                var bar = await context
                    .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                    .SingleAsync<Bar>();
            });
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleOrDefaultAsync(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var bar = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .SingleOrDefaultAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleOrDefaultAsyncIsNull(Func<IDbConnection> dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var bar = await context
                .Command("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                .SingleOrDefaultAsync<Bar>();

            //Then
            Assert.Null(bar);
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
                .Command("select 1")
                .ExecuteAsync<int>();

            var results2 = await context
                .Command("select 'hello, there'")
                .ExecuteAsync<string>();

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
                .Command("select 1")
                .ExecuteAsync<int>();

            var results2 = context
                .Command("select 2")
                .ExecuteAsync<int>();

            var results3 = context
                .Command("select 3")
                .ExecuteAsync<int>();

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
                var connection1 = context.GetOpenConnectionAsync(default);

                var results = await context
                    .Select("1 as alpha")
                    .SingleAsync<Bar>();

                var results2 = await context
                    .Select("2 as alpha")
                    .SingleAsync<Bar>();

                var connection2 = context.GetOpenConnectionAsync(default); ;

                Assert.Same(connection1, connection2);

                Assert.Equal(1, results.Alpha);
                Assert.Equal(2, results2.Alpha);
                transaction.Commit();
            }

            var results3 = await context
                .Select("3 as alpha")
                .SingleAsync<Bar>();
            Assert.Equal(3, results3.Alpha);

            using (var transaction = await context.BeginTransactionAsync())
            {
                var results4 = await context
                    .Select("4 as alpha")
                    .SingleAsync<Bar>();

                //Then
                Assert.Equal(4, results4.Alpha);

                // TRANSACTION NOT COMMITTED
            };

            var results5 = await context
                    .Select("5 as alpha")
                    .SingleAsync<Bar>();
            Assert.Equal(5, results5.Alpha);
        }

        public static IEnumerable<object[]> GetDbConnection()
        {
            yield return new object[] { (Func<IDbConnection>)(() => new NpgsqlConnection("Host=localhost;Database=postgres;Username=postgres;Password=toadstool")) };
            yield return new object[] { (Func<IDbConnection>)(() => new SqlConnection("Server=localhost;Uid=sa;Pwd=Toadstool123")) };
        }
    }
}