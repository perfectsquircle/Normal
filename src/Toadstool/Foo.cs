using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toadstool
{
    public class Foo
    {
        private IDbContextProvider _dbContextProvider;

        public async Task<IEnumerable<Bar>> GetBars()
        {
            IDbContext context = await _dbContextProvider.GetDbContextAsync();

            var results = context
                .Query("select a,b,c from bar where foo = @foo")
                .WithParameter("foo", 2)
                .Execute()
                .As<Bar>();

            return results;
        }

        public class Bar
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
        }
    }
}