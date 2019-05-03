using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toadstool
{
    public static class IDbContextExtensions
    {
        public static IDbCommandBuilder Query(this IDbContext context, IQueryBuilder query)
        {
            return context.Query(query.Build()).WithParameters(query.Parameters);
        }

        public static Task<IList<T>> AsListOf<T>(this IDbContext context, IQueryBuilder query)
        {
            return context
                .Query(query.Build())
                .WithParameters(query.Parameters)
                .AsListOf<T>();
        }

        public static Task<int> ExecuteAsync(this IDbContext context, IQueryBuilder query)
        {
            return context
                .Query(query.Build())
                .WithParameters(query.Parameters)
                .ExecuteNonQueryAsync();
        }

        public static Task<object> ExecuteScalarAsync(this IDbContext context, IQueryBuilder query)
        {
            return context
                .Query(query.Build())
                .WithParameters(query.Parameters)
                .ExecuteScalarAsync();
        }

        public static SelectBuilder Select(this IDbContext context, params string[] selectList)
        {
            return new SelectBuilder(selectList, context);
        }

        public static InsertBuilder InsertInto(this IDbContext context, string tableName, params string[] columnNames)
        {
            return new InsertBuilder(tableName, columnNames, context);
        }
    }
}