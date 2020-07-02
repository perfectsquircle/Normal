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
    public class WideWorldImportersTests
    {
        private Database _postgresDatabase;
        private Database _sqlServerDatabase;

        public WideWorldImportersTests()
        {
            _postgresDatabase = new Database(c =>
            {
                c.UseConnection<NpgsqlConnection>("Host=localhost;Database=wide_world_importers_pg;Username=postgres;Password=normal");
                c.UseLogging(Helpers.GetLogger());
                c.UseCaching(Helpers.GetMemoryCache());
            });

            _sqlServerDatabase = new Database(c =>
            {
                c.UseConnection<SqlConnection>("Server=localhost;Uid=sa;Pwd=Normal123;Database=WideWorldImporters");
                c.UseLogging(Helpers.GetLogger());
                c.UseCaching(Helpers.GetMemoryCache());
            });
        }

        public static IEnumerable<object[]> GetSelectTestCases()
        {
            yield return new object[] { true, "SELECT stock_item_id, stock_item_name FROM warehouse.stock_items ORDER BY stock_item_id LIMIT 10" };
            yield return new object[] { false, "SELECT TOP 10 StockItemID, StockItemName FROM Warehouse.StockItems ORDER BY StockItemID" };
        }

        [Theory]
        [MemberData(nameof(GetSelectTestCases))]
        public async Task ShouldSelectFromStockItems(bool isPostgres, string query)
        {
            //Given
            var database = isPostgres ? _postgresDatabase : _sqlServerDatabase;

            //When
            var results = await database
                .CreateCommand(query)
                .ToListAsync<StockItem>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(10, results.Count);
            var first = results.First();
            Assert.Equal(1, first.StockItemID);
            Assert.Equal("USB missile launcher (Green)", first.StockItemName);
            var last = results.Last();
            Assert.Equal(10, last.StockItemID);
            Assert.Equal("USB food flash drive - chocolate bar", last.StockItemName);
            Assert.Equal("USB food flash drive - chocolate bar", last.StockItemName);
        }

        [Theory]
        [MemberData(nameof(GetSelectTestCases))]
        public async Task ShouldSelectEnumerableFromStockItems(bool isPostgres, string query)
        {
            //Given
            var database = isPostgres ? _postgresDatabase : _sqlServerDatabase;

            //When
            var results = await database
                .CreateCommand(query)
                .ToEnumerableAsync<StockItem>();

            //Then
            results = results.ToList();
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(10, results.Count());
            var first = results.First();
            Assert.Equal(1, first.StockItemID);
            Assert.Equal("USB missile launcher (Green)", first.StockItemName);
            var last = results.Last();
            Assert.Equal(10, last.StockItemID);
            Assert.Equal("USB food flash drive - chocolate bar", last.StockItemName);
            Assert.Equal("USB food flash drive - chocolate bar", last.StockItemName);
        }

        public static IEnumerable<object[]> GetSelectWithParametersTestCases()
        {
            yield return new object[] { true, "SELECT stock_item_id, stock_item_name FROM warehouse.stock_items WHERE supplier_id = @supplierId AND tax_rate = @taxRate ORDER BY stock_item_id" };
            yield return new object[] { false, "SELECT StockItemID, StockItemName FROM Warehouse.StockItems WHERE SupplierId = @supplierId AND TaxRate = @taxRate  ORDER BY StockItemID" };
        }

        [Theory]
        [MemberData(nameof(GetSelectWithParametersTestCases))]
        public async Task ShouldSelectFromStockItemsWithParameters(bool isPostgres, string query)
        {
            //Given
            var database = isPostgres ? _postgresDatabase : _sqlServerDatabase;

            //When
            var results = await database
                .CreateCommand(query)
                .WithParameter("supplierId", 2)
                .WithParameter("brand", null) // not in query
                .WithParameter("taxRate", 15.0)
                .ToListAsync<StockItem>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(3, results.Count);
            var first = results.First();
            Assert.Equal(150, first.StockItemID);
            Assert.Equal("Pack of 12 action figures (variety)", first.StockItemName);
            var last = results.Last();
            Assert.Equal(152, last.StockItemID);
            Assert.Equal("Pack of 12 action figures (female)", last.StockItemName);
        }

        public static IEnumerable<object[]> GetSelectBooleanTestCases()
        {
            yield return new object[] { true, "SELECT stock_item_id, stock_item_name, is_chiller_stock FROM warehouse.stock_items WHERE is_chiller_stock = true" };
            yield return new object[] { false, "SELECT StockItemID, StockItemName, IsChillerStock FROM Warehouse.StockItems WHERE IsChillerStock = 1" };
        }

        [Theory]
        [MemberData(nameof(GetSelectBooleanTestCases))]
        public async Task ShouldSelectBoolean(bool isPostgres, string query)
        {
            //Given
            var database = isPostgres ? _postgresDatabase : _sqlServerDatabase;

            //When
            var results = await database
                .CreateCommand(query)
                .ToListAsync<StockItem>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.All(results, si => { Assert.True(si.IsChillerStock); });
        }

        public static IEnumerable<object[]> GetShouldHandleNullableIntTestCases()
        {
            yield return new object[] { true, "SELECT stock_item_id, stock_item_name, color_id FROM warehouse.stock_items WHERE color_id is null" };
            yield return new object[] { false, "SELECT StockItemID, StockItemName, ColorId FROM Warehouse.StockItems WHERE ColorId is null" };
        }

        [Theory]
        [MemberData(nameof(GetShouldHandleNullableIntTestCases))]
        public async Task ShouldHandleNullableInt(bool isPostgres, string query)
        {
            //Given
            var database = isPostgres ? _postgresDatabase : _sqlServerDatabase;

            //When
            var results = await database
                .CreateCommand(query)
                .ToListAsync<StockItem>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.All(results, si => { Assert.Null(si.ColorId); });
        }

        [Fact]
        public async Task MultipleQueriesInTransaction()
        {
            //Given
            var database = _postgresDatabase;

            //When
            using (var transaction = database.BeginTransaction())
            {
                Assert.Same(transaction, database.CurrentTransaction);
                const string cityName = "Calvinville";
                var results0 = await database
                    .DeleteFrom("application.cities")
                    .Where("city_name").EqualTo(cityName)
                    .ExecuteNonQueryAsync();
                Assert.Same(transaction, database.CurrentTransaction);
                var results1 = await database
                    .InsertInto("application.cities")
                    .Columns("city_name", "state_province_id", "last_edited_by")
                    .Values(cityName, 1, 1)
                    .ExecuteNonQueryAsync();
                Assert.Same(transaction, database.CurrentTransaction);
                var results2 = await database
                    .Select("city_name")
                    .From("application.cities")
                    .Where("city_name").EqualTo(cityName)
                    .SingleAsync<string>();

                Assert.Equal(1, results1);
                Assert.Equal(cityName, results2);
                transaction.Commit();
            }
            Assert.Null(database.CurrentTransaction);
        }

        [Fact]
        public async Task ShouldCacheResults()
        {
            //Given
            var database = _postgresDatabase;

            Func<int, Task<double>> getResults = async (supplierId) =>
            {
                return await database
                    .CreateCommand("select random()")
                    .WithParameter("supplierId", supplierId)
                    .CacheFor(TimeSpan.FromMinutes(1))
                    .FirstAsync<double>();
            };

            //When
            var results1 = await getResults(1);
            var results2 = await getResults(1);
            var results3 = await getResults(1);

            //Then
            Assert.Equal(results1, results2);
            Assert.Equal(results2, results3);
        }

        [Fact]
        public async Task ShouldNotCacheResults()
        {
            //Given
            var database = _postgresDatabase;

            Func<int, Task<double>> getResults = async (supplierId) =>
            {
                return await database
                    .CreateCommand("select random()")
                    .WithParameter("supplierId", supplierId)
                    .CacheFor(TimeSpan.FromMinutes(1))
                    .FirstAsync<double>();
            };

            //When
            var results1 = await getResults(2);
            var results2 = await getResults(3);
            var results3 = await getResults(4);

            //Then
            Assert.NotEqual(results1, results2);
            Assert.NotEqual(results2, results3);
        }
    }
}