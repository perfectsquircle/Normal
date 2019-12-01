using System.Threading;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using Npgsql;
using Xunit;

namespace Normal.UnitTests
{
    public class DbContextTests
    {
        [Fact]
        public async Task ShouldOnlyEnlistOnce()
        {
            //Given
            var createConnectionCalled = 0;
            var connection = new Mock<DbConnection>();
            connection.DefaultValue = DefaultValue.Mock;
            var dbContext = new DbContext(() => { createConnectionCalled++; return connection.Object; });

            //When
            using (var transaction = dbContext.BeginTransaction())
            {
                var task1 = dbContext.Select("1").ExecuteNonQueryAsync();
                var task2 = dbContext.Select("1").ExecuteNonQueryAsync();
                var task3 = dbContext.Select("1").ExecuteNonQueryAsync();

                await Task.WhenAll(task1, task2, task3);
            }

            //Then
            Assert.Equal(1, createConnectionCalled);
        }

        [Fact]
        public async Task ShouldBeConstructable()
        {
            //Given
            var dbContext = new DbContext(typeof(NpgsqlConnection), "Host=localhost;Database=postgres;Username=postgres;Password=normal");

            //When
            var connection = await dbContext.GetOpenConnectionAsync(default(CancellationToken));

            //Then
            Assert.NotNull(connection);
            Assert.NotNull(connection.DbConnection);
            Assert.Null(connection.DbTransaction);
        }
    }
}