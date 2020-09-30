using System;
using System.Data;
using Npgsql;
using Xunit;

namespace Normal.UnitTests
{
    public class CommandBuilderTests : IDisposable
    {
        private readonly Connection _connection;

        public CommandBuilderTests()
        {
            var connection = new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=postgres;Password=normal");
            connection.Open();
            _connection = new Connection(connection);
        }

        [Fact]
        public void ShouldBeConstructable()
        {
            //Given

            //When
            var builder = new CommandBuilder();

            //Then
            Assert.NotNull(builder);
        }

        [Theory]
        [InlineData("banana", 123, CommandType.Text)]
        [InlineData("", 9000, CommandType.StoredProcedure)]
        public void ShouldBeBuildable(string commandText, int commandTimeout, CommandType commandType)
        {
            //Given
            var builder = new CommandBuilder()
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
            var command = (builder as CommandBuilder).Build(_connection);

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

        public void Dispose()
        {
            _connection.Dispose(true);
        }
    }
}