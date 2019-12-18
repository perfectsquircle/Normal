using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    public static class IDbContextExtensions
    {
        public static ISelectBuilder Select(this IDbContext context, params string[] selectList)
        {
            return new SelectBuilder(context).WithColumns(selectList);
        }

        public static ISelectBuilder Select<T>(this IDbContext context)
        {
            var table = new Table(typeof(T));
            return context.Select(table.ColumnNames.ToArray()).From(table.Name);
        }

        public static async Task<IEnumerable<T>> SelectAsync<T>(this IDbContext context, CancellationToken cancellationToken = default)
        {
            return await context.Select<T>().ToListAsync<T>(cancellationToken);
        }

        public static async Task<T> SelectAsync<T>(this IDbContext context, object id, CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            return await context.Select<T>()
                .Where(table.PrimaryKeyColumnName).EqualTo(id)
                .FirstOrDefaultAsync<T>(cancellationToken);
        }

        public static IInsertBuilder InsertInto(this IDbContext context, string tableName)
        {
            return new InsertBuilder(context).WithTableName(tableName);
        }

        public static Task<int> InsertAsync<T>(this IDbContext context, T model, CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            var columns = table.GetColumns(model);
            return context.InsertInto(table.Name)
                .Columns(columns.Keys.ToArray())
                .Values(columns.Values.ToArray())
                .ExecuteNonQueryAsync(cancellationToken);
        }

        public static IUpdateBuilder Update(this IDbContext context, string tableName)
        {
            return new UpdateBuilder(context).WithTableName(tableName);
        }

        public static Task<int> UpdateAsync<T>(this IDbContext context, T model, CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            var columns = table.GetColumns(model);
            var (primaryKey, primaryKeyValue) = table.GetPrimaryKey(model);
            columns.Remove(primaryKey);
            return context.Update(table.Name)
                .Set(columns)
                .Where(primaryKey).EqualTo(primaryKeyValue)
                .ExecuteNonQueryAsync(cancellationToken);
        }

        public static IDeleteBuilder DeleteFrom(this IDbContext context, string tableName)
        {
            return new DeleteBuilder(context).WithTableName(tableName);
        }

        public static Task<int> DeleteAsync<T>(this IDbContext context, T model, CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            var (primaryKey, primaryKeyValue) = table.GetPrimaryKey(model);
            return context.DeleteFrom(table.Name)
                .Where(primaryKey).EqualTo(primaryKeyValue)
                .ExecuteNonQueryAsync(cancellationToken);
        }

        public static IDbCommandBuilder CreateCommandFromFile(this IDbContext context, string fileName, Encoding encoding = default)
        {
            encoding = encoding ?? Encoding.Default;
            var file = File.ReadAllText(fileName, encoding);
            return context.CreateCommand(file);
        }

        public static IDbCommandBuilder CreateCommandFromResource(this IDbContext context, string resourceName, Assembly inputAssembly = default, Encoding encoding = default)
        {
            encoding = encoding ?? Encoding.Default;
            using (var stream = FindResourceFromAssemblies(resourceName, inputAssembly, Assembly.GetCallingAssembly(), Assembly.GetEntryAssembly()))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Could not find resource named: " + resourceName);
                }
                using (var reader = new StreamReader(stream, encoding))
                {
                    return context.CreateCommand(reader.ReadToEnd());
                }
            }
        }

        private static Stream FindResourceFromAssemblies(string resourceName, params Assembly[] inputAssemblies)
        {
            foreach (var assembly in inputAssemblies)
            {
                if (assembly == null)
                {
                    continue;
                }
                var resource = assembly
                    .GetManifestResourceNames()
                    .FirstOrDefault(r => r.EndsWith(resourceName)); ;
                if (resource != null)
                {
                    return assembly.GetManifestResourceStream(resource);
                }
            }
            return null;
        }
    }
}