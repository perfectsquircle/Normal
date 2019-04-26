using Xunit;
using static Toadstool.SqlBuilder;

namespace Toadstool.UnitTests
{
    public class SelectBuilderTests
    {
        [Fact]
        public void ShouldCreateSelectBuilder()
        {
            //Given

            //When
            var actual = Select("a, b, c");

            //Then
            Assert.NotNull(actual);
            Assert.IsType<SelectBuilder>(actual);
        }

        [Fact]
        public void ShouldBeBuildable()
        {
            //Given
            var expected =
@"SELECT customer_id, first_name, last_name
FROM customers c
JOIN orders o on o.customer_id = c.customer_id
WHERE c.first_name is not null
AND c.last_name is not null
ORDER BY c.last_name
LIMIT 100";

            //When
            var actual = Select("customer_id", "first_name", "last_name")
                .From("customers c")
                .Join("orders o on o.customer_id = c.customer_id")
                .Where("c.first_name is not null")
                .And("c.last_name is not null")
                .OrderBy("c.last_name")
                .Limit(100)
                .Build();

            //Then
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }
    }
}