using System.Data.Common;
using System.Threading.Tasks;
using Moq;
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
                var task1 = dbContext.Select("1").ExecuteAsync();
                var task2 = dbContext.Select("1").ExecuteAsync();
                var task3 = dbContext.Select("1").ExecuteAsync();

                await Task.WhenAll(task1, task2, task3);
            }

            //Then
            Assert.Equal(1, createConnectionCalled);
        }
    }
}