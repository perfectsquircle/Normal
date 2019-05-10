using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public static class IDbContextExtensions
    {
        public static IDbCommandBuilder Query(this IDbContext context, IStatementBuilder statement)
        {
            return context.Query(statement.Build()).WithParameters(statement.Parameters);
        }

        public static SelectBuilder Select(this IDbContext context, params string[] selectList)
        {
            return new SelectBuilder(selectList, context);
        }

        public static InsertBuilder InsertInto(this IDbContext context, string tableName, params string[] columnNames)
        {
            return new InsertBuilder(tableName, columnNames, context);
        }

        public static UpdateBuilder Update(this IDbContext context, string tableName)
        {
            return new UpdateBuilder(tableName, context);
        }

        public static DeleteBuilder DeleteFrom(this IDbContext context, string tableName)
        {
            return new DeleteBuilder(tableName, context);
        }
    }
}