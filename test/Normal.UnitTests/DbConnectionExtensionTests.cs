using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace Normal.UnitTests
{
    public class DbConnectionExtensionTests
    {
        private readonly string _postgresConnection = "Host=localhost;Database=wide_world_importers_pg;Username=postgres;Password=normal";
        [Fact]
        public async Task ShouldSelect()
        {
            //Given
            using (var connection = new NpgsqlConnection(_postgresConnection))
            {
                //When
                var result = await connection
                    .Select("1")
                    .SingleAsync<int>();
                var result2 = await connection
                    .Select("2")
                    .SingleAsync<int>();
                var result3 = await connection
                    .Select("3")
                    .SingleAsync<int>();

                //Then
                Assert.Equal(1, result);
                Assert.Equal(2, result2);
                Assert.Equal(3, result3);
            }
        }
    }
}