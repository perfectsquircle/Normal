using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        [Theory]
        [MemberData(nameof(GetSelectTestCases))]
        public async Task ShouldSelectFromStockItems(Func<IDbConnection> dbConnection, string query)
        {
            //Given
            var context = new DbContext()
                .WithConnection(dbConnection);

            //When
            var results = await context
                .Query(query)
                .AsListOf<StockItem>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(10, results.Count);
            Assert.Equal(1, results[0].StockItemID);
            Assert.Equal("USB missile launcher (Green)", results[0].StockItemName);
            Assert.Equal(10, results[9].StockItemID);
            Assert.Equal("USB food flash drive - chocolate bar", results[9].StockItemName);
        }

        public static IEnumerable<object[]> GetSelectTestCases()
        {
            yield return new object[]
            {
                _postgresConnection,
                "SELECT stock_item_id, stock_item_name FROM warehouse.stock_items ORDER BY stock_item_id LIMIT 10"
            };
            yield return new object[] {
                _sqlServerConnection,
                "SELECT TOP 10 StockItemID, StockItemName FROM Warehouse.StockItems ORDER BY StockItemID"
            };
        }

    }
}