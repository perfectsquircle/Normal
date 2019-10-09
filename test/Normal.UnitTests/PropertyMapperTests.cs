using System;
using System.Data;
using Moq;
using Xunit;

namespace Normal.UnitTests
{
    public class PropertyMapperTests
    {
        private readonly Mock<IDataRecord> _dataRecord;

        public PropertyMapperTests()
        {
            _dataRecord = new Mock<IDataRecord>(MockBehavior.Strict);
            _dataRecord.Setup(dr => dr.GetInt16(0)).Returns(16);
            _dataRecord.Setup(dr => dr.GetInt32(0)).Returns(32);
            _dataRecord.Setup(dr => dr.GetInt64(0)).Returns(64L);
            _dataRecord.Setup(dr => dr.GetFloat(0)).Returns(32f);
            _dataRecord.Setup(dr => dr.GetDouble(0)).Returns(64.0);
        }

        [Theory]
        [InlineData(typeof(short), (short)16)]
        [InlineData(typeof(int), 32)]
        [InlineData(typeof(long), 64L)]
        [InlineData(typeof(float), 32f)]
        [InlineData(typeof(double), 64.0)]
        [InlineData(typeof(short?), (short)16)]
        [InlineData(typeof(int?), 32)]
        [InlineData(typeof(long?), 64L)]
        [InlineData(typeof(float?), 32f)]
        [InlineData(typeof(double?), 64.0)]
        public void ShouldMapToCorrectNumeric(Type propertyType, object expectedValue)
        {
            //Given
            var propertyMapper = new PropertyMapper()
                .WithPropertyType(propertyType);
            var columnReader = propertyMapper.CreateColumnReader();

            //When
            var result = columnReader.Invoke(_dataRecord.Object);

            //Then
            Assert.Equal(expectedValue, result);
        }
    }
}