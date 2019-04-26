namespace Toadstool
{
    public static class IDbContextExtensions
    {
        public static IDbCommandBuilder Query(this IDbContext context, IBuildableQuery query)
        {
            return context.Query(query.Build()).WithParameters(query.Parameters);
        }

        public static IDbCommandBuilder Query(this DbContext context, IBuildableQuery query)
        {
            return context.Query(query.Build()).WithParameters(query.Parameters);
        }

        public static SelectBuilder Select(this DbContext context, params string[] selectList)
        {
            return new SelectBuilder(selectList, context);
        }
    }
}