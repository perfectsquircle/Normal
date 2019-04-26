using Xunit;
using static Toadstool.SqlBuilder;

namespace Toadstool.UnitTests
{
    public class SelectBuilderTests
    {
        [Fact]
        public void ShouldBuildSelect()
        {
            //Given

            //When
            var result = Select("a, b, c");

            //Then
            Assert.NotNull(result);
            Assert.IsType<SelectBuilder>(result);
        }
    }
}