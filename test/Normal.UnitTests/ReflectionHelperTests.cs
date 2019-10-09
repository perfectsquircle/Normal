using System;
using Xunit;

namespace Normal.UnitTests
{
    public class ReflectionHelperTests
    {
        [Fact]
        public void ShouldConvertObjectToDictionary()
        {
            //Given
            var target = new
            {
                Foo = "Bar",
                Cheese = 12,
                Zamboni = DateTimeOffset.Now
            };

            //When
            var result = ReflectionHelper.ToDictionary(target);

            //Then
            Assert.NotNull(result);
            Assert.Equal(target.Foo, result["Foo"]);
            Assert.Equal(target.Cheese, result["Cheese"]);
            Assert.Equal(target.Zamboni, result["Zamboni"]);
        }
    }
}