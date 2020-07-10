using System.Threading;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using Npgsql;
using Xunit;

namespace Normal.UnitTests
{
    public class DatabaseTests
    {
        [Fact]
        public async Task ShouldOnlyEnlistOnce()
        {
            //Given
            var createConnectionCalled = 0;
            var connection = new Mock<DbConnection>();
            connection.DefaultValue = DefaultValue.Mock;
            var database = new Database(() => { createConnectionCalled++; return connection.Object; });

            //When
            using (var transaction = database.BeginTransaction())
            {
                var task1 = database.Select("1").ExecuteNonQueryAsync();
                var task2 = database.Select("1").ExecuteNonQueryAsync();
                var task3 = database.Select("1").ExecuteNonQueryAsync();

                await Task.WhenAll(task1, task2, task3);
            }

            //Then
            Assert.Equal(1, createConnectionCalled);
        }

        [Fact]
        public async Task ShouldBeConstructable()
        {
            //Given
            var database = Database.WithConnection<NpgsqlConnection>("Host=localhost;Database=postgres;Username=postgres;Password=normal");

            //When
            var connection = await database.GetOpenConnectionAsync(default(CancellationToken));

            //Then
            Assert.NotNull(connection);
            Assert.NotNull(connection.DbConnection);
            Assert.Null(connection.DbTransaction);
        }
    }
}