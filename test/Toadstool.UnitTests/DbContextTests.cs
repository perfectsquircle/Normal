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
            public int Alpha { get; set; }
            public string Beta { get; set; }
            public string Charlie { get; } = "Can't set me";
        }

        [Fact]
        public async Task AsShouldWork()
        {
            //Given
            var context = new DbContext()
                .WithConnection(new NpgsqlConnection("Host=localhost;Database=cfurano;"));

            //When
            var results = await context
                .Query("select 7 as alpha, 'foo' as beta, 'something' as charlie, true as delta")
                .ExecuteAsync()
                .AsEnumerableOf<Bar>();

            //Then
            Assert.NotNull(results);
            // Assert.NotEmpty(results); // This iterates the IEnumerable, thus consuming it.
            var bar = results.Single();
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }

        [Fact]
        public async Task AsListShouldWork()
        {
            //Given
            var context = new DbContext()
                .WithConnection(new NpgsqlConnection("Host=localhost;Database=cfurano;"));

            //When
            var results = await context
                .Query("select 7 as alpha, 'foo' as beta, 'something' as charlie, true as delta")
                .ExecuteAsync()
                .AsListOf<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            var bar = results.Single();
            Assert.Equal(7, bar.Alpha);
            Assert.Equal("foo", bar.Beta);
            Assert.Equal("Can't set me", bar.Charlie);
        }
    }
}