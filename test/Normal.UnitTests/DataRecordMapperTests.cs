using System;
using System.Collections.Generic;
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
                    Name = "bravo", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "delta", Value = 3.14159,
                },
                new {
                    Name = "createDate", Value = now,
               },
               new {
                   Name = "tango", Value = 7
               },
               new {
                   Name = "Whisky", Value = "BAR"
               }
            });
            var mapper = new ClassDataRecordMapper<Bar>();

            //When
            var result = mapper.MapDataRecord(record.Object);

            //Then
            Assert.NotNull(result);
            Assert.Equal(777, result.Alpha);
            Assert.Equal("Your lucky day", result.Bravo);
            Assert.Equal(now, result.CreateDate);
            Assert.Equal("Can't set me", result.Charlie);
            Assert.Equal("3.14159", result.Delta);
            Assert.Equal(TheEnum.bat, result.Tango);
            Assert.Equal(TheEnum.bar, result.Whisky);
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
                    Name = "bravo", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "createDate", Value = now,
               }
            });
            var mapper = new DynamicDataRecordMapper<dynamic>();

            //When
            var result = mapper.MapDataRecord(record.Object);

            //Then
            Assert.NotNull(result);
            Assert.Equal(777, result.alpha);
            Assert.Equal("Your lucky day", result.bravo);
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
                    Name = "bravo", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "createDate", Value = now,
               }
            });
            var mapper = new PrimitiveDataRecordMapper<int>();

            //When
            var result = mapper.MapDataRecord(record.Object);

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
                    Name = "bravo", Value = "Your lucky day",
                },
                new {
                    Name = "charlie", Value = "Punk",
                },
                new {
                    Name = "createDate", Value = now,
               }
            });
            var mapper = new PrimitiveDataRecordMapper<string>();

            //When
            var result = mapper.MapDataRecord(record.Object);

            //Then
            Assert.Equal("Your lucky day", result);
        }

        [Theory]
        [MemberData(nameof(MapTypeTestCases))]
        public void ShouldMapCorrectType<T>(T instance, Type expectedMapperType)
        {
            //Given
            var factory = new DataRecordMapperFactory();

            //When
            var mapper = factory.CreateMapper<T>();

            //Then
            Assert.NotNull(mapper);
            Assert.Equal(expectedMapperType, mapper.GetType());
        }

        public static IEnumerable<object[]> MapTypeTestCases()
        {
            yield return new object[] { default(bool), typeof(PrimitiveDataRecordMapper<bool>) };
            yield return new object[] { default(byte), typeof(PrimitiveDataRecordMapper<byte>) };
            yield return new object[] { default(short), typeof(PrimitiveDataRecordMapper<short>) };
            yield return new object[] { default(ushort), typeof(PrimitiveDataRecordMapper<ushort>) };
            yield return new object[] { default(int), typeof(PrimitiveDataRecordMapper<int>) };
            yield return new object[] { default(uint), typeof(PrimitiveDataRecordMapper<uint>) };
            yield return new object[] { default(long), typeof(PrimitiveDataRecordMapper<long>) };
            yield return new object[] { default(ulong), typeof(PrimitiveDataRecordMapper<ulong>) };
            yield return new object[] { default(float), typeof(PrimitiveDataRecordMapper<float>) };
            yield return new object[] { default(double), typeof(PrimitiveDataRecordMapper<double>) };
            yield return new object[] { default(decimal), typeof(PrimitiveDataRecordMapper<decimal>) };

            yield return new object[] { new byte[0], typeof(PrimitiveDataRecordMapper<byte[]>) };
            yield return new object[] { new char[0], typeof(PrimitiveDataRecordMapper<char[]>) };

            yield return new object[] { "Hello, world!", typeof(PrimitiveDataRecordMapper<string>) };
            yield return new object[] { default(DateTime), typeof(PrimitiveDataRecordMapper<DateTime>) };
            yield return new object[] { default(DateTimeOffset), typeof(PrimitiveDataRecordMapper<DateTimeOffset>) };
            yield return new object[] { default(TheEnum), typeof(PrimitiveDataRecordMapper<TheEnum>) };

            yield return new object[] { new Bar(), typeof(ClassDataRecordMapper<Bar>) };
            yield return new object[] { new StockItem(), typeof(ClassDataRecordMapper<StockItem>) };
            yield return new object[] { new TheStruct(), typeof(ClassDataRecordMapper<TheStruct>) };
            yield return new object[] { (dynamic)new object(), typeof(DynamicDataRecordMapper<dynamic>) };
        }
    }
}