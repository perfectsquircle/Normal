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
            var context = new SqlServerContext("slcsql-qa.solarcity.local");

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
            var context = new PostgresContext("User ID=incentives_api;Password=BB8DC0F7-F196-425C-A8FE-EEF0606AFC67;Host=sjc04d1mtpdb01.teslamotors.com;Database=incentive_reference;");

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