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
            List<Bar> results = await context
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
            List<Bar> results = await context
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
                Assert.NotNull(context._activeDbConnectionContext);
                var connection1 = context._activeDbConnectionContext.DbConnection;
                var transaction1 = context._activeDbConnectionContext.DbTransaction;

                var results = await context
                    .Select("1")
                    .ExecuteAsync<int>();

                var results2 = await context
                    .Select("2")
                    .ExecuteAsync<int>();

                var connection2 = context._activeDbConnectionContext.DbConnection;
                var transaction2 = context._activeDbConnectionContext.DbTransaction;

                Assert.Same(connection1, connection2);
                Assert.Same(transaction1, transaction2);

                Assert.Equal(1, results);
                Assert.Equal(2, results2);
                Assert.False(transaction.IsComplete);
                transaction.Commit();
                Assert.True(transaction.IsComplete);
            }
            Assert.Null(context._activeDbConnectionContext);

            var results3 = await context
                    .Select("3")
                    .ExecuteAsync<int>();
            Assert.Equal(3, results3);

            using (var transaction = await context.BeginTransactionAsync())
            {
                Assert.NotNull(context._activeDbConnectionContext);

                var results = await context
                    .Select("4")
                    .ExecuteAsync<int>();

                //Then
                Assert.Equal(4, results);

                // TRANSACTION NOT COMMITTED
            };
            Assert.Null(context._activeDbConnectionContext);

            var results4 = await context
                    .Command("select 6")
                    .ExecuteAsync<int>();
            Assert.Equal(6, results4);
        }

        public static IEnumerable<object[]> GetDbConnection()
        {
            yield return new object[] { (Func<IDbConnection>)(() => new NpgsqlConnection("Host=localhost;Database=postgres;Username=postgres;Password=toadstool")) };
            yield return new object[] { (Func<IDbConnection>)(() => new SqlConnection("Server=localhost;Uid=sa;Pwd=Toadstool123")) };
        }
    }
}