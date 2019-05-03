using System;
using System.Data;
using Npgsql;
using Xunit;
using static Toadstool.StatementBuilder;

namespace Toadstool.UnitTests
{
    public class UpdateBuilderTests
    {
        private static Func<IDbConnection> _postgresConnection = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=toadstool;Password=toadstool");

        [Fact]
        public void ShouldCreateUpdateBuilder()
        {
            //Given

            //When
            var actual = Update("flark");

            //Then
            Assert.NotNull(actual);
            Assert.IsType<UpdateBuilder>(actual);
        }

        [Fact]
        public void ShouldBeBuildable()
        {
            //Given
            var expected =
@"UPDATE flarktown
SET banana = 3
WHERE foo_bar = @toadstool_parameter_1";

            //When
            var actual = Update("flarktown")
                .Set("banana = 3")
                .Where("foo_bar").EqualTo("goodbye")
                .Build();

            //Then
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldBeBuildable2()
        {
            //Given
            var expected =
@"UPDATE flarktown
SET banana = @toadstool_parameter_1, foo_bar = @toadstool_parameter_2
WHERE foo_bar = @toadstool_parameter_3";

            //When
            var actual = Update("flarktown")
                .Set(
                    "banana", 3,
                    "foo_bar", "hello"
                )
                .Where("foo_bar").EqualTo("goodbye")
                .Build();

            //Then
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}