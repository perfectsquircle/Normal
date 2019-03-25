using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using Xunit;

namespace Toadstool.UnitTests
{
    public class DbCommandBuilderTests
    {
        private readonly Mock<DbContext> _context;
        public DbCommandBuilderTests()
        {
            var command = new Mock<IDbCommand>()
                .SetupAllProperties();
            var connection = new Mock<IDbConnection>()
                .SetupAllProperties();
            connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);
            _context = new Mock<DbContext>()
                .SetupAllProperties();
            _context
                .Setup(c => c.GetOpenConnectionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(connection.Object);
        }

        [Fact]
        public void ShouldBeConstructable()
        {
            //Given

            //When
            var builder = new DbCommandBuilder();

            //Then
            Assert.NotNull(builder);
        }

        [Fact]
        public async Task ShouldNotBeBuildable()
        {
            //Given

            //When
            var builder = new DbCommandBuilder();

            //Then
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await builder.BuildAsync());
        }

        [Theory, AutoData]
        public async Task ShouldBeBuildable(string commandText, int commandTimeout, CommandType commandType)
        {
            //Given
            var builder = new DbCommandBuilder()
                .WithDbContext(_context.Object)
                .WithCommandText(commandText)
                .WithCommandTimeout(commandTimeout)
                .WithCommandType(commandType)
                ;

            //When
            var command = await builder.BuildAsync();

            //Then
            Assert.NotNull(command);
            Assert.Equal(commandText, command.CommandText);
            Assert.Equal(commandTimeout, command.CommandTimeout);
            Assert.Equal(commandType, command.CommandType);
        }
    }
}