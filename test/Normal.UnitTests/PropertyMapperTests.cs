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
        }

        [Theory]
        [InlineData(typeof(short), (short)16, (short)16)]
        [InlineData(typeof(int), (short)16, (int)16)]
        [InlineData(typeof(long), (short)16, (long)16)]
        [InlineData(typeof(float), (short)16, (float)16)]
        [InlineData(typeof(double), (short)16, (double)16)]
        [InlineData(typeof(short), (double)64, (short)64)]
        public void ShouldMapToCorrectNumeric(Type propertyType, object columnValue, object expectedValue)
        {
            //Given
            _dataRecord.Setup(dr => dr[0]).Returns(columnValue);
            var propertyMapper = new PropertyMapper()
                .WithColumnType(columnValue.GetType())
                .WithPropertyType(propertyType);
            var columnReader = propertyMapper.CreateColumnReader();

            //When
            var result = columnReader.Invoke(_dataRecord.Object);

            //Then
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void ShouldMapToEnum()
        {
            _dataRecord.Setup(dr => dr[0]).Returns(1);
            var propertyMapper = new PropertyMapper()
                .WithColumnType(typeof(int))
                .WithPropertyType(typeof(TheEnum));
            var columnReader = propertyMapper.CreateColumnReader();

            //When
            var result = columnReader.Invoke(_dataRecord.Object);

            //Then
            Assert.Equal(TheEnum.bar, result);
        }

        [Fact]
        public void ShouldMapToEnumFromString()
        {
            _dataRecord.Setup(dr => dr.GetString(0)).Returns("bar");
            var propertyMapper = new PropertyMapper()
                .WithColumnType(typeof(string))
                .WithPropertyType(typeof(TheEnum));
            var columnReader = propertyMapper.CreateColumnReader();

            //When
            var result = columnReader.Invoke(_dataRecord.Object);

            //Then
            Assert.Equal(TheEnum.bar, result);
        }

        [Theory]
        [InlineData("foo", typeof(string))]
        [InlineData(123, typeof(int))]
        public void ShouldMapMatchingType(object sourceObject, Type destType)
        {
            //Given
            _dataRecord.Setup(dr => dr[0]).Returns(sourceObject);
            var propertyMapper = new PropertyMapper()
                .WithColumnType(sourceObject.GetType())
                .WithPropertyType(destType);
            var columnReader = propertyMapper.CreateColumnReader();

            //When
            var result = columnReader.Invoke(_dataRecord.Object);

            //Then
            Assert.NotNull(result);
            Assert.IsType(destType, result);
        }

        [Theory]
        [InlineData(123, typeof(string), "123")]
        public void ShouldMapMismatchedType(object sourceObject, Type destType, object expectedValue)
        {
            //Given
            _dataRecord.Setup(dr => dr[0]).Returns(sourceObject);
            var propertyMapper = new PropertyMapper()
                .WithColumnType(sourceObject.GetType())
                .WithPropertyType(destType);
            var columnReader = propertyMapper.CreateColumnReader();

            //When
            var result = columnReader.Invoke(_dataRecord.Object);

            //Then
            Assert.NotNull(result);
            Assert.IsType(destType, result);
            Assert.Equal(expectedValue, result);
        }
    }
}