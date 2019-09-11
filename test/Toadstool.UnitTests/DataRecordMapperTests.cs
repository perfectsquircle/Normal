using System;
using Moq;
using Moq.DataExtensions;
using Toadstool.UnitTests.Fixtures;
using Xunit;

namespace Toadstool.UnitTests
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
    }
}