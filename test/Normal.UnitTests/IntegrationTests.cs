using System;
using System.Collections.Generic;
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
        public async Task ToEnumerableAsync(Database database)
        {
            //Given

            //When
            var results = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta")
                .ToEnumerableAsync<Bar>();

            //Then
            results = results.ToList();
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            var bar = results.Single();
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Bravo);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task ToEnumerableAsyncIsEmpty(Database database)
        {
            //Given

            //When
            var results = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta where 1 = 2")
                .ToEnumerableAsync<Bar>();

            //Then
            results = results.ToList();
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task ToListAsync(Database database)
        {
            //Given

            //When
            var results = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta")
                .ToListAsync<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            var bar = results.Single();
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Bravo);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task ToListAsyncIsEmpty(Database database)
        {
            //Given

            //When
            var results = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta where 1 = 2")
                .ToListAsync<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstAsync(Database database)
        {
            //Given

            //When
            var bar = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta")
                .FirstAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Bravo);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstAsyncThrows(Database database)
        {
            //Given

            //When
            await Assert.ThrowsAsync<System.InvalidOperationException>(async () =>
            {
                var bar = await database
                    .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta where 1 = 2")
                    .FirstAsync<Bar>();
            });
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstOrDefaultAsync(Database database)
        {
            //Given

            //When
            var bar = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta")
                .FirstOrDefaultAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Bravo);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task FirstOrDefaultAsyncIsNull(Database database)
        {
            //Given

            //When
            var bar = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta where 1 = 2")
                .FirstOrDefaultAsync<Bar>();

            //Then
            Assert.Null(bar);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleAsync(Database database)
        {
            //Given

            //When
            var bar = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta")
                .SingleAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Bravo);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleAsyncThrows(Database database)
        {
            //Given

            //When
            await Assert.ThrowsAsync<System.InvalidOperationException>(async () =>
            {
                var bar = await database
                    .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta where 1 = 2")
                    .SingleAsync<Bar>();
            });
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleOrDefaultAsync(Database database)
        {
            //Given

            //When
            var bar = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta")
                .SingleOrDefaultAsync<Bar>();

            //Then
            Assert.NotNull(bar);
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Bravo);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SingleOrDefaultAsyncIsNull(Database database)
        {
            //Given

            //When
            var bar = await database
                .CreateCommand("select 7 as alpha, 'foo' as bravo, 'something' as charlie, 'delta' as delta where 1 = 2")
                .SingleOrDefaultAsync<Bar>();

            //Then
            Assert.Null(bar);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task MultipleQueries(Database database)
        {
            //Given

            //When
            var results = await database
                .CreateCommand("select 1")
                .SingleAsync<int>();

            var results2 = await database
                .CreateCommand("select 'hello, there'")
                .SingleAsync<string>();

            //Then
            Assert.Equal(1, results);
            Assert.Equal("hello, there", results2);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task SimultaneousQueries(Database database)
        {
            //Given

            //When
            var results = database
                .CreateCommand("select 1")
                .SingleAsync<int>();

            var results2 = database
                .CreateCommand("select 2")
                .SingleAsync<int>();

            var results3 = database
                .CreateCommand("select 3")
                .SingleAsync<int>();

            //Then
            Assert.Equal(1, await results);
            Assert.Equal(2, await results2);
            Assert.Equal(3, await results3);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task MultipleQueriesInTransaction(Database database)
        {
            //Given

            //When
            using (var transaction = database.BeginTransaction())
            {
                Assert.Same(transaction, database.CurrentTransaction);
                var results = await database
                    .Select("1 as alpha")
                    .SingleAsync<Bar>();
                Assert.Same(transaction, database.CurrentTransaction);
                var results2 = await database
                    .Select("2 as alpha")
                    .SingleAsync<Bar>();
                Assert.Same(transaction, database.CurrentTransaction);
                Assert.Equal(1, results.Alpha);
                Assert.Equal(2, results2.Alpha);
                transaction.Commit();

                var resultsZ = await database
                    .Select("1 as alpha")
                    .SingleAsync<Bar>();
            }
            Assert.Null(database.CurrentTransaction);

            var results3 = await database
                .Select("3 as alpha")
                .SingleAsync<Bar>();
            Assert.Equal(3, results3.Alpha);

            using (var transaction = database.BeginTransaction())
            {
                var results4 = await database
                    .Select("4 as alpha")
                    .SingleAsync<Bar>();

                //Then
                Assert.Equal(4, results4.Alpha);

                // TRANSACTION NOT COMMITTED
            };

            var results5 = await database
                    .Select("5 as alpha")
                    .SingleAsync<Bar>();
            Assert.Equal(5, results5.Alpha);
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public void NestedTransactionThrows(Database database)
        {
            //Given

            //When
            Assert.Throws<InvalidOperationException>(() =>
            {
                var transaction = database.BeginTransaction();
                var transaction2 = database.BeginTransaction();
            });
        }

        [Theory]
        [MemberData(nameof(GetDbConnection))]
        public async Task CreateCommandFromResource(Database database)
        {
            //Given

            //When
            IList<dynamic> results = await database
                .CreateCommandFromResource("Foo.sql")
                .ToListAsync<dynamic>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            var bar = results.Single();
            Assert.Equal(1, bar.x);
            Assert.Equal(2, bar.y);
            Assert.Equal(3, bar.z);
        }

        public static IEnumerable<object[]> GetDbConnection()
        {
            yield return new object[] { Database.WithConnection<NpgsqlConnection>("Host=localhost;Database=postgres;Username=postgres;Password=normal") };
            yield return new object[] { Database.WithConnection<SqlConnection>("Server=localhost;Uid=SA;Pwd=Normal123") };
        }
    }
}