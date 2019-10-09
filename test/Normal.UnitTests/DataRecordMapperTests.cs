using System;
using Moq;
using Moq.DataExtensions;
using Normal.UnitTests.Fixtures;
using Xunit;

namespace Normal.UnitTests
{
    public class DataRecordMapperTests
    {
        [Fact]
        public void ShouldMapDataRecord()
        {
            //Given
            var repository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            var now = DateTimeOffset.Now;
            var record = repository.CreateIDataRecord(new object[] {
                new {
                    Name = "alpha", Value = 777,
                },
                new {
                    Name = "beta", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "createDate", Value = now,
               }
            });
            var mapper = new ClassDataRecordMapper();

            //When
            var result = mapper.MapDataRecord<Bar>(record.Object);

            //Then
            Assert.NotNull(result);
            Assert.Equal(777, result.Alpha);
            Assert.Equal("Your lucky day", result.Beta);
            Assert.Equal(now, result.CreateDate);
            Assert.Equal("Can't set me", result.Charlie);
        }

        [Fact]
        public void ShouldMapToDynamic()
        {
            //Given
            var repository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            var now = DateTimeOffset.Now;
            var record = repository.CreateIDataRecord(new object[] {
                new {
                    Name = "alpha", Value = 777,
                },
                new {
                    Name = "beta", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "createDate", Value = now,
               }
            });
            var mapper = new DynamicDataRecordMapper();

            //When
            var result = mapper.MapDataRecord<dynamic>(record.Object);

            //Then
            Assert.NotNull(result);
            Assert.Equal(777, result.alpha);
            Assert.Equal("Your lucky day", result.beta);
            Assert.Equal("Punk", result.charlie);
            Assert.Equal(now, result.createDate);
        }

        [Fact]
        public void ShouldMapPrimitiveType()
        {
            //Given
            var repository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            var now = DateTimeOffset.Now;
            var record = repository.CreateIDataRecord(new object[] {
                new {
                    Name = "alpha", Value = 777,
                },
                new {
                    Name = "beta", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "createDate", Value = now,
               }
            });
            var mapper = new PrimitiveDataRecordMapper();

            //When
            var result = mapper.MapDataRecord<int>(record.Object);

            //Then
            Assert.Equal(777, result);
        }

        [Fact]
        public void ShouldMapString()
        {
            //Given
            var repository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            var now = DateTimeOffset.Now;
            var record = repository.CreateIDataRecord(new object[] {
                new {
                    Name = "beta", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "createDate", Value = now,
               }
            });
            var mapper = new PrimitiveDataRecordMapper();

            //When
            var result = mapper.MapDataRecord<string>(record.Object);

            //Then
            Assert.Equal("Your lucky day", result);
        }

        [Theory]
        [InlineData(typeof(bool), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(byte), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(short), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(ushort), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(int), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(uint), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(int?), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(long), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(ulong), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(long?), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(float), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(double), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(decimal), typeof(PrimitiveDataRecordMapper))]

        [InlineData(typeof(byte[]), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(char[]), typeof(PrimitiveDataRecordMapper))]

        [InlineData(typeof(string), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(DateTime), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(DateTime?), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(DateTimeOffset), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(DateTimeOffset?), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(TheEnum), typeof(PrimitiveDataRecordMapper))]
        [InlineData(typeof(TheEnum?), typeof(PrimitiveDataRecordMapper))]

        [InlineData(typeof(Bar), typeof(ClassDataRecordMapper))]
        [InlineData(typeof(StockItem), typeof(ClassDataRecordMapper))]
        [InlineData(typeof(TheStruct), typeof(ClassDataRecordMapper))]
        [InlineData(typeof(TheStruct?), typeof(ClassDataRecordMapper))]
        [InlineData(typeof(object), typeof(DynamicDataRecordMapper))]
        public void ShouldMapCorrectType(Type type, Type expectedMapperType)
        {
            //Given
            var factory = new DataRecordMapperFactory();

            //When
            var mapper = factory.CreateMapper(type);

            //Then
            Assert.NotNull(mapper);
            Assert.Equal(expectedMapperType, mapper.GetType());
        }
    }
}