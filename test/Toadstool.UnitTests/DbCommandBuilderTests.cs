using System;
using System.Data.Common;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Xunit;

namespace Toadstool.UnitTests
{
    public class DbCommandBuilderTests
    {
        private readonly Fixture _fixture;
        public DbCommandBuilderTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void ShouldBeConstructable()
        {
            //Given

            //When
            var builder = new DbCommandBuilder();

            //Then
            Assert.NotNull(builder.Parameters);
        }

        [Fact]
        public void ShouldBeConstructableWithContext()
        {
            //Given
            var context = _fixture.Create<DbContext>();

            //When
            var builder = new DbCommandBuilder()
                .WithDbContext(context);

            //Then
            Assert.NotNull(builder.Parameters);
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

        [Fact]
        public async Task ShouldBeBuildable()
        {
            //Given
            var connection = _fixture.Create<DbConnection>();
            var context = _fixture.Create<DbContext>()
                .WithConnection(connection);
            var builder = new DbCommandBuilder()
                .WithDbContext(context);

            //When
            var command = await builder.BuildAsync();

            //Then
            Assert.NotNull(command);
            Assert.Equal(context.Connection.ConnectionString, command.Connection.ConnectionString);
        }
    }
}