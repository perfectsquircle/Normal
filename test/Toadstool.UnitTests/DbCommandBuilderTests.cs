using System;
using System.Data;
using Moq;
using Moq.DataExtensions;
using Xunit;

namespace Toadstool.UnitTests
{
    public class DbCommandBuilderTests
    {
        private readonly Mock<IDbConnectionWrapper> _connection;

        public DbCommandBuilderTests()
        {
            var repository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            _connection = repository.Create<IDbConnectionWrapper>()
                .SetupAllProperties();
            _connection.Setup(c => c.CreateCommand())
                .Returns(() => repository.CreateIDbCommand().Object);
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

        [Theory]
        [InlineData("banana", 123, CommandType.Text)]
        [InlineData("", -100, CommandType.StoredProcedure)]
        public void ShouldBeBuildable(string commandText, int commandTimeout, CommandType commandType)
        {
            //Given
            var builder = new DbCommandBuilder()
                .WithCommandText(commandText)
                .WithCommandTimeout(commandTimeout)
                .WithCommandType(commandType)
                .WithParameter("Free Beer", "Yay")
                .WithParameters(new
                {
                    Free = "Beer"
                })
                ;

            //When
            var command = (builder as DbCommandBuilder).Build(_connection.Object);

            //Then
            Assert.NotNull(command);
            Assert.Equal(commandText, command.CommandText);
            Assert.Equal(commandTimeout, command.CommandTimeout);
            Assert.Equal(commandType, command.CommandType);
            Assert.Equal("Free Beer", (command.Parameters[0] as IDbDataParameter).ParameterName);
            Assert.Equal("Yay", (command.Parameters[0] as IDbDataParameter).Value);
            Assert.Equal("Free", (command.Parameters[1] as IDbDataParameter).ParameterName);
            Assert.Equal("Beer", (command.Parameters[1] as IDbDataParameter).Value);
        }
    }
}