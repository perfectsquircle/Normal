using Xunit;

namespace Normal.UnitTests
{
    public class DeleteBuilderTests
    {

        [Fact]
        public void ShouldCreateDeleteBuilder()
        {
            //Given

            //When
            var actual = new DeleteBuilder("flark");

            //Then
            Assert.NotNull(actual);
            Assert.IsType<DeleteBuilder>(actual);
        }

        [Fact]
        public void ShouldBeBuildable()
        {
            //Given
            var expected =
@"DELETE FROM flarktown
WHERE foo_bar = @normal_1";

            //When
            var actual = new DeleteBuilder("flarktown")
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
@"DELETE FROM flarktown
WHERE foo_bar = @normal_1
AND bar_bat > @normal_2";

            //When
            var actual = new DeleteBuilder("flarktown")
                .Where("foo_bar").EqualTo("goodbye")
                .And("bar_bat").GreaterThan(0)
                .Build();

            //Then
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}