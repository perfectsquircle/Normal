using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace Toadstool.UnitTests
{
    public class DbContextTests
    {
        [Fact(Skip = "Support later")]
        public async Task SqlServerContextShouldWork()
        {
            //Given
            var context = new DbContext()
                .WithConnection(new SqlConnection("???"));

            //When

            //Then
        }

        public class Bar
        {
            public int A { get; set; }
        }

        [Fact]
        public async Task AsShouldWork()
        {
            //Given
            var context = new DbContext()
                .WithConnection(new NpgsqlConnection("Host=localhost;Database=cfurano;"));

            //When
            var results = await context
                .Query("select 7 as A")
                .WithParameter("foo", "bar")
                .ExecuteAsync()
                .As<Bar>();

            //Then
            Assert.NotNull(results);
            // Assert.NotEmpty(results); // This iterates the IEnumerable, thus consuming it.
            Assert.Equal(7, results.Single().A);
        }

        [Fact]
        public async Task AsListShouldWork()
        {
            //Given
            var context = new DbContext()
                .WithConnection(new NpgsqlConnection("Host=localhost;Database=cfurano;"));

            //When
            var results = await context
                .Query("select 7 as A")
                .WithParameter("foo", "bar")
                .ExecuteAsync()
                .AsList<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(7, results.Single().A);
        }
    }
}