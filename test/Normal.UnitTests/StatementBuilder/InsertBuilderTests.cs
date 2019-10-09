using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace Normal.UnitTests
{
    public class InsertBuilderTests
    {
        private static CreateConnection _postgresConnection = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=normal;Password=normal");

        [Fact]
        public void ShouldBeBuildable()
        {
            //Given
            var expected =
@"INSERT INTO customer
(first_name, last_name, age)
VALUES
(@normal_1, @normal_2, @normal_3)";

            //When
            var actual = new InsertBuilder("customer")
                .Columns("first_name", "last_name", "age")
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
@"INSERT INTO customer
(first_name, last_name, age)
VALUES
(@normal_1, @normal_2, @normal_3)
,(@normal_4, @normal_5, @normal_6)
,(@normal_7, @normal_8, @normal_9)";

            //When
            var actual = new InsertBuilder("customer")
                .Columns("first_name", "last_name", "age")
                .Values("Jose", "Cuervo", 100)
                .Values("Jose", "Cuervo", 200)
                .Values("Jose", "Cuervo", 300)
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
                .InsertInto("warehouse.colors")
                .Columns("color_name", "last_edited_by")
                .Values("Gurple", 1)
                .ExecuteAsync();

            var gurple = await context
                .Select("color_name")
                .From("warehouse.colors")
                .Where("color_name").EqualTo("Gurple")
                .ExecuteAsync<string>();

            //Then
            Assert.Equal(1, rowsInserted);
            Assert.Equal("Gurple", gurple);
        }
    }
}