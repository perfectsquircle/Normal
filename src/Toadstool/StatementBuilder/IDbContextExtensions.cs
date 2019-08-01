namespace Toadstool
{
    public static class IDbContextExtensions
    {
        public static ISelectBuilder Select(this IDbContext context, params string[] selectList)
        {
            return new SelectBuilder(selectList)
                .WithContext(context) as SelectBuilder;
        }

        public static IInsertBuilder InsertInto(this IDbContext context, string tableName)
        {
            return new InsertBuilder(tableName)
                .WithContext(context) as InsertBuilder;
        }

        public static IUpdateBuilder Update(this IDbContext context, string tableName)
        {
            return new UpdateBuilder(tableName)
                .WithContext(context) as UpdateBuilder;
        }

        public static IDeleteBuilder DeleteFrom(this IDbContext context, string tableName)
        {
            return new DeleteBuilder(tableName)
                .WithContext(context) as DeleteBuilder;
        }
    }
}