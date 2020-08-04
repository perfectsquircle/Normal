using System.Data;
using System.Data.Common;

namespace Normal
{
    public static class DbConnectionExtensions
    {
        public static ISelectBuilder Select(this DbConnection dbConnection, params string[] selectList)
        {
            return new SelectBuilder(new SingleConnectionDatabase(dbConnection)).WithColumns(selectList);
        }
    }
}