using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Normal.UnitTests.Fixtures;
using Xunit;

namespace Normal.UnitTests
{
    public class IntegrationTests
    {
        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task ToListAsync(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            IList<Bar> results = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
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
        public async Task ToListAsyncIsEmpty(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            IList<Bar> results = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                .ToListAsync<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstAsync(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var bar = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .FirstAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstAsyncThrows(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            await Assert.ThrowsAsync<System.InvalidOperationException>(async () =>
            {
                var bar = await context
                    .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                    .FirstAsync<Bar>();
            });
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstOrDefaultAsync(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var bar = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .FirstOrDefaultAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstOrDefaultAsyncIsNull(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var bar = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                .FirstOrDefaultAsync<Bar>();

            //Then
            Assert.Null(bar);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleAsync(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var bar = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .SingleAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleAsyncThrows(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            await Assert.ThrowsAsync<System.InvalidOperationException>(async () =>
            {
                var bar = await context
                    .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                    .SingleAsync<Bar>();
            });
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleOrDefaultAsync(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var bar = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta")
                .SingleOrDefaultAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleOrDefaultAsyncIsNull(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var bar = await context
                .CreateCommand("select 7 as alpha, 'foo' as beta, 'something' as charlie, 'delta' as delta where 1 = 2")
                .SingleOrDefaultAsync<Bar>();

            //Then
            Assert.Null(bar);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task MultipleQueries(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var results = await context
                .CreateCommand("select 1")
                .ExecuteAsync<int>();

            var results2 = await context
                .CreateCommand("select 'hello, there'")
                .ExecuteAsync<string>();

            //Then
            Assert.Equal(1, results);
            Assert.Equal("hello, there", results2);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SimultaneousQueries(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            var results = context
                .CreateCommand("select 1")
                .ExecuteAsync<int>();

            var results2 = context
                .CreateCommand("select 2")
                .ExecuteAsync<int>();

            var results3 = context
                .CreateCommand("select 3")
                .ExecuteAsync<int>();

            //Then
            Assert.Equal(1, await results);
            Assert.Equal(2, await results2);
            Assert.Equal(3, await results3);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task MultipleQueriesInTransaction(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            using (var transaction = context.BeginTransaction())
            {
                Assert.Same(transaction, context.CurrentTransaction);
                var results = await context
                    .Select("1 as alpha")
                    .SingleAsync<Bar>();
                Assert.Same(transaction, context.CurrentTransaction);
                var results2 = await context
                    .Select("2 as alpha")
                    .SingleAsync<Bar>();
                Assert.Same(transaction, context.CurrentTransaction);
                Assert.Equal(1, results.Alpha);
                Assert.Equal(2, results2.Alpha);
                transaction.Commit();

                var resultsZ = await context
                    .Select("1 as alpha")
                    .SingleAsync<Bar>();
            }
            Assert.Null(context.CurrentTransaction);

            var results3 = await context
                .Select("3 as alpha")
                .SingleAsync<Bar>();
            Assert.Equal(3, results3.Alpha);

            using (var transaction = context.BeginTransaction())
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

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public void NestedTransactionThrows(CreateConnection dbConnection)
        {
            //Given
            var context = new DbContext()
                .WithCreateConnection(dbConnection);

            //When
            Assert.Throws<InvalidOperationException>(() =>
            {
                var transaction = context.BeginTransaction();
                var transaction2 = context.BeginTransaction();
            });
        }

        public static IEnumerable<object[]> GetDbConnection()
        {
            yield return new object[] { (CreateConnection)(() => new NpgsqlConnection("Host=localhost;Database=postgres;Username=postgres;Password=normal")) };
            yield return new object[] { (CreateConnection)(() => new SqlConnection("Server=localhost;Uid=SA;Pwd=Normal123")) };
        }
    }
}