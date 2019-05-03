using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using Xunit;
using static Toadstool.StatementBuilder;

namespace Toadstool.UnitTests
{
    public class InsertBuilderTests
    {
        private static Func<IDbConnection> _postgresConnection = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=toadstool;Password=toadstool");

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

        [Fact]
        public async Task ShouldInsert()
        {
            //Given
            var context = new DbContext(_postgresConnection);

            //When
            var rowsInserted = await context
                .InsertInto("warehouse.colors", "color_name", "last_edited_by")
                .Values("Gurple", 1)
                .ExecuteAsync();

            var gurple = await context
                .Select("color_name")
                .From("warehouse.colors")
                .Where("color_name").EqualTo("Gurple")
                .ExecuteScalarAsync<string>();

            //Then
            Assert.Equal(1, rowsInserted);
            Assert.Equal("Gurple", gurple);
        }
    }
}