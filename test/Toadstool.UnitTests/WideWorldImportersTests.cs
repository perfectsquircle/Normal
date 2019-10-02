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
    public class WideWorldImportersTests
    {
        private static Func<IDbConnection> _postgresConnection = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=postgres;Password=toadstool");
        private static Func<IDbConnection> _sqlServerConnection = () => new SqlConnection("Server=localhost;Uid=sa;Pwd=Toadstool123;Database=WideWorldImporters");

        public static IEnumerable<object[]> GetSelectTestCases()
        {
            yield return new object[] { _postgresConnection, "SELECT stock_item_id, stock_item_name FROM warehouse.stock_items ORDER BY stock_item_id LIMIT 10" };
            yield return new object[] { _sqlServerConnection, "SELECT TOP 10 StockItemID, StockItemName FROM Warehouse.StockItems ORDER BY StockItemID" };
        }

        [Theory]
        [MemberData(nameof(GetSelectTestCases))]
        public async Task ShouldSelectFromStockItems(Func<IDbConnection> dbConnection, string query)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Command(query)
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

        public static IEnumerable<object[]> GetSelectWithParametersTestCases()
        {
            yield return new object[] { _postgresConnection, "SELECT stock_item_id, stock_item_name FROM warehouse.stock_items WHERE supplier_id = @supplierId AND tax_rate = @taxRate ORDER BY stock_item_id" };
            yield return new object[] { _sqlServerConnection, "SELECT StockItemID, StockItemName FROM Warehouse.StockItems WHERE SupplierId = @supplierId AND TaxRate = @taxRate  ORDER BY StockItemID" };
        }

        [Theory]
        [MemberData(nameof(GetSelectWithParametersTestCases))]
        public async Task ShouldSelectFromStockItemsWithParameters(Func<IDbConnection> dbConnection, string query)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Command(query)
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
            yield return new object[] { _postgresConnection, "SELECT stock_item_id, stock_item_name, is_chiller_stock FROM warehouse.stock_items WHERE is_chiller_stock = true" };
            yield return new object[] { _sqlServerConnection, "SELECT StockItemID, StockItemName, IsChillerStock FROM Warehouse.StockItems WHERE IsChillerStock = 1" };
        }

        [Theory]
        [MemberData(nameof(GetSelectBooleanTestCases))]
        public async Task ShouldSelectBoolean(Func<IDbConnection> dbConnection, string query)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Command(query)
                .ToListAsync<StockItem>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.All(results, si => { Assert.True(si.IsChillerStock); });
        }

        public static IEnumerable<object[]> GetShouldHandleNullableIntTestCases()
        {
            yield return new object[] { _postgresConnection, "SELECT stock_item_id, stock_item_name, color_id FROM warehouse.stock_items WHERE color_id is null" };
            yield return new object[] { _sqlServerConnection, "SELECT StockItemID, StockItemName, ColorId FROM Warehouse.StockItems WHERE ColorId is null" };
        }

        [Theory]
        [MemberData(nameof(GetShouldHandleNullableIntTestCases))]
        public async Task ShouldHandleNullableInt(Func<IDbConnection> dbConnection, string query)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Command(query)
                .ToListAsync<StockItem>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.All(results, si => { Assert.Null(si.ColorId); });
        }
    }
}