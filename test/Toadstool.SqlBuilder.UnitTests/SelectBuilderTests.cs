using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using Toadstool.UnitTests.Fixtures;
using Xunit;
using static Toadstool.SqlBuilder;

namespace Toadstool.UnitTests
{
    public class SelectBuilderTests
    {
        private static Func<IDbConnection> _postgresConnection = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=postgres;Password=toadstool");

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
JOIN orders o on o.customer_id = c.customer_id
WHERE c.first_name is not null
AND c.last_name is not null
ORDER BY c.last_name
LIMIT 100";

            //When
            var actual = Select("customer_id", "first_name", "last_name")
                .From("customers c")
                .Join("orders o on o.customer_id = c.customer_id")
                .Where("c.first_name is not null")
                .And("c.last_name is not null")
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
            // SELECT stock_item_id, stock_item_name FROM warehouse.stock_items WHERE supplier_id = @supplierId AND tax_rate = @taxRate ORDER BY stock_item_id
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var query = Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .Where("supplier_id = @supplierId")
                .And("tax_rate = @taxRate")
                .OrderBy("stock_item_id");

            var results = await context
                .Query(query)
                .WithParameter("supplierId", 2)
                .WithParameter("brand", null) // not in query
                .WithParameter("taxRate", 15.0)
                .AsListOf<StockItem>();

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
            // SELECT stock_item_id, stock_item_name FROM warehouse.stock_items WHERE supplier_id = @supplierId AND tax_rate = @taxRate ORDER BY stock_item_id
            var context = new DbContext().WithConnection(_postgresConnection);

            //When
            var results = await context
                .Select("stock_item_id", "stock_item_name")
                .From("warehouse.stock_items")
                .WhereEqual("supplier_id", 2)
                .AndEqual("tax_rate", 15.0)
                .OrderBy("stock_item_id")
                .Query()
                .AsListOf<StockItem>();

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
    }
}