using Xunit;

namespace Toadstool.UnitTests
{
    public class UpdateBuilderTests
    {

        [Fact]
        public void ShouldCreateUpdateBuilder()
        {
            //Given

            //When
            var actual = new UpdateBuilder("flark");

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
SET banana = @toadstool_1
WHERE foo_bar = @toadstool_2";

            //When
            var actual = new UpdateBuilder("flarktown")
                .Set("banana").EqualTo(3)
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
SET banana = @toadstool_1
, foo_bar = @toadstool_2
WHERE foo_bar = @toadstool_3";

            //When
            var actual = new UpdateBuilder("flarktown")
                .Set("banana").EqualTo(3)
                .Set("foo_bar").EqualTo("hello")
                .Where("foo_bar").EqualTo("goodbye")
                .Build();

            //Then
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}