using System.Data;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using Xunit;

namespace Toadstool.UnitTests
{
    public class DbCommandBuilderTests
    {
        private readonly Mock<IDbConnection> _connection;

        public DbCommandBuilderTests()
        {
            var command = new Mock<IDbCommand>()
                .SetupAllProperties();
            _connection = new Mock<IDbConnection>()
                .SetupAllProperties();
            _connection.Setup(c => c.CreateCommand())
                .Returns(command.Object);
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

        [Theory, AutoData]
        public void ShouldBeBuildable(string commandText, int commandTimeout, CommandType commandType)
        {
            //Given
            var builder = new DbCommandBuilder()
                .WithCommandText(commandText)
                .WithCommandTimeout(commandTimeout)
                .WithCommandType(commandType)
                ;

            //When
            var command = builder.Build(new DbConnectionWrapper(_connection.Object));

            //Then
            Assert.NotNull(command);
            Assert.Equal(commandText, command.CommandText);
            Assert.Equal(commandTimeout, command.CommandTimeout);
            Assert.Equal(commandType, command.CommandType);
        }
    }
}