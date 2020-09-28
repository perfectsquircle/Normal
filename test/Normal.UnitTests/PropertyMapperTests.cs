using System;
using System.Data;
using Moq;
using Normal.UnitTests.Fixtures;
using Xunit;

namespace Normal.UnitTests
{
    public class PropertyMapperTests
    {
        private readonly Mock<IDataRecord> _dataRecord;

        public PropertyMapperTests()
        {
            _dataRecord = new Mock<IDataRecord>(MockBehavior.Strict);
            _dataRecord.Setup(dr => dr.IsDBNull(0)).Returns(false);
        }

        [Theory]
        [InlineData(default(short), (short)16, (short)16)]
        [InlineData(default(int), (short)16, (int)16)]
        [InlineData(default(long), (short)16, (long)16)]
        [InlineData(default(float), (short)16, (float)16)]
        [InlineData(default(double), (short)16, (double)16)]
        [InlineData(default(short), (double)64, (short)64)]
        public void ShouldMapToCorrectNumeric<T>(T instance, object columnValue, object expectedValue)
        {
            //Given
            _dataRecord.Setup(dr => dr.GetValue(0)).Returns(columnValue);
            var columnReader = MemberMatch.GetColumnReader<T>(columnValue.GetType(), 0);

            //When
            var result = columnReader(_dataRecord.Object);

            //Then
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void ShouldMapToEnum()
        {
            _dataRecord.Setup(dr => dr.GetValue(0)).Returns(1);
            var columnReader = MemberMatch.GetColumnReader<TheEnum>(typeof(int), 0);

            //When
            var result = columnReader(_dataRecord.Object);

            //Then
            Assert.Equal(TheEnum.bar, result);
        }

        [Fact]
        public void ShouldMapToEnumFromString()
        {
            _dataRecord.Setup(dr => dr.GetString(0)).Returns("bar");
            var columnReader = MemberMatch.GetColumnReader<TheEnum>(typeof(string), 0);

            //When
            var result = columnReader(_dataRecord.Object);

            //Then
            Assert.Equal(TheEnum.bar, result);
        }

        [Theory]
        [InlineData("foo", "Hello, world!")]
        [InlineData(123, default(int))]
        public void ShouldMapMatchingType<T>(object sourceObject, T destType)
        {
            //Given
            _dataRecord.Setup(dr => dr.GetValue(0)).Returns(sourceObject);
            var columnReader = MemberMatch.GetColumnReader<T>(sourceObject.GetType(), 0);

            //When
            var result = columnReader(_dataRecord.Object);

            //Then
            Assert.NotNull(result);
            Assert.IsType<T>(result);
        }

        [Theory]
        [InlineData(123, "Hello, world!", "123")]
        public void ShouldMapMismatchedType<T>(object sourceObject, T destType, object expectedValue)
        {
            //Given
            _dataRecord.Setup(dr => dr.GetValue(0)).Returns(sourceObject);
            var columnReader = MemberMatch.GetColumnReader<T>(sourceObject.GetType(), 0);

            //When
            var result = columnReader.Invoke(_dataRecord.Object);

            //Then
            Assert.NotNull(result);
            Assert.IsType<T>(result);
            Assert.Equal(expectedValue, result);
        }
    }
}