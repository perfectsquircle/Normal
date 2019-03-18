using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Toadstool.UnitTests
{
    public class DbContextTests
    {
        [Fact]
        public async Task SqlServerContextShouldWork()
        {
            //Given
            var context = new SqlServerContext("???");

            //When

            //Then
        }

        public class Bar
        {
            public int A { get; set; }
        }

        [Fact]
        public async Task PostgresContextShouldWork()
        {
            //Given
            var context = new PostgresContext("Host=localhost;Database=cfurano;");

            //When
            var results = context
                .Query("select 7 as A")
                .Execute()
                .AsList<Bar>();

            //Then
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(7, results.Single().A);
        }
    }
}