using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using Toadstool.UnitTests.Fixtures;
using Xunit;
using static Toadstool.StatementBuilder;


namespace Toadstool.UnitTests
{
    public class SelectBuilderTests
    {
        private static Func<IDbConnection> _postgresConnection = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=toadstool;Password=toadstool");

        [Fact]
        public void ShouldCreateSelectBuilder()
        {
            //Given

            //When
            var actual = Select("a, b, c");

            //Then
            Assert.NotNull(actual);
            Assert.IsType<SelectBuilder>(actual);
        }

        [Fact]
        public void ShouldBeBuildable()
        {
            //Given
            var expected =
@"SELECT customer_id, first_name, last_name
FROM customers c
JOIN orders o
ON o.customer_id = c.customer_id
WHERE c.first_name IS NOT NULL
AND c.last_name IS NOT NULL
ORDER BY c.last_name
LIMIT 100";

            //When
            var actual = Select("customer_id", "first_name", "last_name")
                .From("customers c")
                .Join("orders o")
                .On("o.customer_id = c.customer_id")
                .Where("c.first_name").IsNotNull()
                .And("c.last_name").IsNotNull()
                .OrderBy("c.last_name")
                .Limit(100)
                .Build();

            //Then
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ShouldBeQueryable()
        {
            //Given
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var query = Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .Where("supplier_id = @supplierId")
                .And("tax_rate = @taxRate").End()
                .OrderBy("stock_item_id");

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

        [Fact]
        public async Task ShouldBeQueryable2()
        {
            //Given
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var results = await context
                .Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .Where("supplier_id").EqualTo(2)
                .And("tax_rate").EqualTo(15.0)
                .OrderBy("stock_item_id")
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

        [Fact]
        public async Task FirstOrDefaultAsync()
        {
            //Given
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var stockItem = await context
                .Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .Where("supplier_id").EqualTo(2)
                .And("tax_rate").EqualTo(15.0)
                .OrderBy("stock_item_id")
                .FirstOrDefaultAsync<StockItem>();

            //Then
            Assert.NotNull(stockItem);
            Assert.Equal(150, stockItem.StockItemID);
            Assert.Equal("Pack of 12 action figures (variety)", stockItem.StockItemName);
        }

        [Fact]
        public async Task FirstAsync()
        {
            //Given
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var stockItem = await context
                .Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .Where("supplier_id").EqualTo(2)
                .And("tax_rate").EqualTo(15.0)
                .OrderBy("stock_item_id")
                .FirstAsync<StockItem>();

            //Then
            Assert.NotNull(stockItem);
            Assert.Equal(150, stockItem.StockItemID);
            Assert.Equal("Pack of 12 action figures (variety)", stockItem.StockItemName);
        }

        [Fact]
        public async Task SingleAsync()
        {
            //Given
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var stockItem = await context
                .Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .Where("stock_item_id").EqualTo(150)
                .SingleAsync<StockItem>();

            //Then
            Assert.NotNull(stockItem);
            Assert.Equal(150, stockItem.StockItemID);
            Assert.Equal("Pack of 12 action figures (variety)", stockItem.StockItemName);
        }

        [Fact]
        public async Task SingleOrDefualtAsync()
        {
            //Given
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var stockItem = await context
                .Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .Where("stock_item_id").EqualTo(999999)
                .SingleOrDefaultAsync<StockItem>();

            //Then
            Assert.Null(stockItem);
        }
    }
}