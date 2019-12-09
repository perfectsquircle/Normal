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
    public class CrudBuilderTests
    {
        private DbContext _postgresContext;

        public CrudBuilderTests()
        {
            _postgresContext = new DbContext(c =>
            {
                c.UseConnection<NpgsqlConnection>("Host=localhost;Database=wide_world_importers_pg;Username=postgres;Password=normal");
                c.UseLogging(Helpers.GetLogger());
                c.UseCaching(Helpers.GetMemoryCache());
            });
        }


        [Fact]
        public async Task ShouldSelectFromStockItems()
        {
            //Given
            var context = _postgresContext;

            //When
            var results = (await context.SelectAsync<StockItemAnnotated>()).ToList();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(227, results.Count);
            var first = results.First();
            Assert.Equal(1, first.StockItemID);
            Assert.Equal("USB missile launcher (Green)", first.StockItemName);
        }

        [Fact]
        public async Task ShouldSelectFromStockItemsWithParameters()
        {
            //Given
            var context = _postgresContext;

            //When
            var results = await context
                .Select<StockItemAnnotated>()
                .Where("supplier_id").EqualTo(2)
                .And("tax_rate").EqualTo(15.0)
                .ToListAsync<StockItemAnnotated>();

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

        [Fact(Skip = "Non-nullable fields")]
        public async Task ShouldInsertIntoStockItems()
        {
            //Given
            var context = _postgresContext;
            var stockItem = new StockItemAnnotated
            {
                StockItemName = "Bananas",
                SupplierId = 2,

            };

            //When
            using (var transaction = context.BeginTransaction())
            {
                var rowsAffected = await context.InsertAsync<StockItemAnnotated>(stockItem);
                transaction.Rollback();

                //Then
                Assert.Equal(1, rowsAffected);
            }
        }
    }
}