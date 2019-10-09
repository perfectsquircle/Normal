using Xunit;

namespace Normal.UnitTests
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
SET banana = @normal_1
WHERE foo_bar = @normal_2";

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
SET banana = @normal_1
, foo_bar = @normal_2
WHERE foo_bar = @normal_3";

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