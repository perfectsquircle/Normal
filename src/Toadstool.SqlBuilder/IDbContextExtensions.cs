namespace Toadstool
{
    public static class IDbContextExtensions
    {
        public static IDbCommandBuilder Query(this IDbContext context, IBuildableQuery sqlBuilder)
        {
            return context.Query(sqlBuilder.Build());
        }

        public static IDbCommandBuilder Query(this DbContext context, IBuildableQuery sqlBuilder)
        {
            return context.Query(sqlBuilder.Build());
        }

        public static SelectBuilder Select(this DbContext context, params string[] selectList)
        {
            return new SelectBuilder(selectList, context);
        }
    }
}