using Xunit;
using static Toadstool.SqlBuilder;

namespace Toadstool.UnitTests
{
    public class InsertBuilderTests
    {
        [Fact]
        public void ShouldBeBuildable()
        {
            //Given
            var expected =
@"INSERT INTO customer (first_name, last_name, age) 
VALUES (@toadstool_parameter_1, @toadstool_parameter_2, @toadstool_parameter_3)";

            //When
            var actual = InsertInto("customer", "first_name", "last_name", "age")
                .Values("Jose", "Cuervo", 100)
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
@"INSERT INTO customer (first_name, last_name, age) 
VALUES (@toadstool_parameter_1, @toadstool_parameter_2, @toadstool_parameter_3),
(@toadstool_parameter_4, @toadstool_parameter_5, @toadstool_parameter_6),
(@toadstool_parameter_7, @toadstool_parameter_8, @toadstool_parameter_9)";

            //When
            var actual = InsertInto("customer", "first_name", "last_name", "age")
                .Values(
                    new object[] { "Jose", "Cuervo", 100 },
                    new object[] { "Jose", "Cuervo", 200 },
                    new object[] { "Jose", "Cuervo", 300 }
                )
                .Build();

            //Then
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}