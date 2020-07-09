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
        private Database _postgresDatabase;
        private Database _sqlServerDatabase;

        public CrudBuilderTests()
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

        [Fact]
        public async Task ShouldSelectFromStockItems()
        {
            //Given
            var database = _postgresDatabase;

            //When
            var results = (await database.SelectAsync<StockItemAnnotated>()).ToList();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(227, results.Count);
            var first = results.First();
            Assert.Equal(1, first.StockItemID);
            Assert.Equal("USB missile launcher (Green)", first.StockItemName);
        }

        [Fact]
        public async Task ShouldSelectFromStockItems_SqlServer()
        {
            //Given
            var database = _sqlServerDatabase;

            //When
            var results = (await database.SelectAsync<StockItem>()).ToList();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(227, results.Count);
            var first = results.First();
            Assert.Equal(1, first.StockItemID);
            Assert.Equal("USB missile launcher (Green)", first.StockItemName);
        }

        [Fact]
        public async Task ShouldSelectFromStockItemsById()
        {
            //Given
            var database = _postgresDatabase;

            //When
            var result = await database.SelectAsync<StockItemAnnotated>(1);

            //Then
            Assert.NotNull(result);
            Assert.Equal(1, result.StockItemID);
            Assert.Equal("USB missile launcher (Green)", result.StockItemName);
        }

        // [Fact]
        // public async Task ShouldSelectFromStockItemsWithParameters()
        // {
        //     //Given
        //     var database = _postgresDatabase;

        //     //When
        //     var results = await database
        //         .Select<StockItemAnnotated>()
        //         .Where("supplier_id").EqualTo(2)
        //         .And("tax_rate").EqualTo(15.0)
        //         .ToListAsync<StockItemAnnotated>();

        //     //Then
        //     Assert.NotNull(results);
        //     Assert.NotEmpty(results);
        //     Assert.Equal(3, results.Count);
        //     var first = results.First();
        //     Assert.Equal(150, first.StockItemID);
        //     Assert.Equal("Pack of 12 action figures (variety)", first.StockItemName);
        //     var last = results.Last();
        //     Assert.Equal(152, last.StockItemID);
        //     Assert.Equal("Pack of 12 action figures (female)", last.StockItemName);
        // }

        [Fact]
        public async Task ShouldInsertIntoStockItems()
        {
            //Given
            var database = _postgresDatabase;
            var stockItem = new StockItemAnnotated
            {
                StockItemName = "Bananas",
                SupplierId = 2,
                ColorId = 1,
                UnitPackageId = 1,
                OuterPackageId = 1,
                LeadTimeDays = 1,
                QuantityPerOuter = 1,
                IsChillerStock = false,
                TaxRate = 1.0m,
                UnitPrice = 1.0m,
                TypicalWeightPerUnit = 1.0m,
                SearchDetails = "Yellow, Fruit",
                LastEditedBy = 1,
            };

            //When
            using (var transaction = database.BeginTransaction())
            {
                var result = await database.InsertAsync<StockItemAnnotated>(stockItem);
                transaction.Rollback();

                //Then
                Assert.NotEqual(0, result.StockItemID);
            }
        }

        [Fact]
        public async Task ShouldUpdateStockItem()
        {
            //Given
            var database = _postgresDatabase;
            var stockItem = new StockItemAnnotated
            {
                StockItemID = 1,
                StockItemName = "Bananas",
                SupplierId = 2,
                ColorId = 1,
                UnitPackageId = 1,
                OuterPackageId = 1,
                LeadTimeDays = 1,
                QuantityPerOuter = 1,
                IsChillerStock = false,
                TaxRate = 1.0m,
                UnitPrice = 1.0m,
                TypicalWeightPerUnit = 1.0m,
                SearchDetails = "Yellow, Fruit",
                LastEditedBy = 1,
            };

            //When
            using (var transaction = database.BeginTransaction())
            {
                var rowsAffected = await database.UpdateAsync(stockItem);
                transaction.Rollback();

                //Then
                Assert.Equal(1, rowsAffected);
            }
        }

        [Fact]
        public async Task ShouldDeleteStockItem()
        {
            //Given
            var database = _postgresDatabase;
            var specialDeal = new SpecialDeal
            {
                SpecialDealId = 1,
            };

            //When
            using (var transaction = database.BeginTransaction())
            {
                var rowsAffected = await database.DeleteAsync<SpecialDeal>(specialDeal);
                transaction.Rollback();

                //Then
                Assert.Equal(1, rowsAffected);
            }
        }
    }
}