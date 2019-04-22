using System.Data.SqlClient;
using System.Threading.Tasks;
using Npgsql;
using Toadstool.UnitTests.Fixtures;
using Xunit;

namespace Toadstool.UnitTests
{
    public class WideWorldImportersTests
    {
        [Fact]
        public async Task ShouldSelectFromStockItems_SqlServer()
        {
            //Given
            var context = new DbContext()
                .WithConnection(() => new SqlConnection("Server=localhost;Uid=sa;Pwd=Toadstool123;Database=WideWorldImporters"));

            //When
            var results = await context
                .Query("SELECT TOP 10 StockItemID, StockItemName FROM Warehouse.StockItems ORDER BY StockItemID")
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

        [Fact]
        public async Task ShouldSelectFromStockItems_Postgres()
        {
            //Given
            var context = new DbContext()
                .WithConnection(() => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=postgres;Password=toadstool"));

            //When
            var results = await context
                .Query("SELECT stock_item_id, stock_item_name FROM warehouse.stock_items ORDER BY stock_item_id LIMIT 10")
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
    }
}