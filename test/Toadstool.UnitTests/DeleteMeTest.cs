using Xunit;

namespace Tesla.Toadstool.UnitTests
{
    public class DeleteMeTest
    {
        [Fact]
        public void ShouldSayBar()
        {
            // Arrange
            var deleteMe = new DeleteMe();

            // Act
            var result = deleteMe.Foo();

            // Assert
            Assert.Equal("bar", result);
        }
    }
}