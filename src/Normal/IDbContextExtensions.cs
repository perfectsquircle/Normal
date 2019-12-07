using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FastMember;

namespace Normal
{
    public static class IDbContextExtensions
    {
        public static ISelectBuilder Select(this IDbContext context, params string[] selectList)
        {
            return new SelectBuilder(selectList)
                .WithContext(context) as SelectBuilder;
        }

        public static ISelectBuilder Select<T>(this IDbContext context)
        {
            var targetType = typeof(T);
            var typeAccessor = TypeAccessor.Create(targetType);
            var tableNameAttribute = targetType.GetCustomAttribute(typeof(TableNameAttribute)) as TableNameAttribute;
            var tableName = tableNameAttribute?.TableName ?? targetType.Name;
            var columnNames = typeAccessor
                .GetMembers()
                .Select(m =>
                {
                    var columnNameAttribute = m.GetAttribute(typeof(ColumnNameAttribute), false) as ColumnNameAttribute;
                    if (columnNameAttribute != null)
                    {
                        return columnNameAttribute.ColumnName;
                    }
                    return m.Name;
                });
            return new SelectBuilder(columnNames.ToArray())
                .From(tableName);
        }

        public static async Task<IEnumerable<T>> SelectAsync<T>(this IDbContext context)
        {
            return await Select<T>(context).ToListAsync<T>();
        }

        public static async Task<T> SelectAsync<T>(this IDbContext context, object id)
        {

            var targetType = typeof(T);
            var typeAccessor = TypeAccessor.Create(targetType);
            var primaryKeyMember = typeAccessor
                .GetMembers()
                .FirstOrDefault(m =>
                {
                    return m.GetAttribute(typeof(PrimaryKeyAttribute), false) != null;
                });

            var primaryKey = primaryKeyMember.Name;

            return await Select<T>(context)
                .Where(primaryKey).EqualTo(id)
                .FirstOrDefaultAsync<T>();
        }

        public static IInsertBuilder InsertInto(this IDbContext context, string tableName)
        {
            return new InsertBuilder(tableName)
                .WithContext(context) as InsertBuilder;
        }

        public static Task<int> InsertAsync<T>(this IDbContext context, T model)
        {
            throw new NotImplementedException();
        }

        public static IUpdateBuilder Update(this IDbContext context, string tableName)
        {
            return new UpdateBuilder(tableName)
                .WithContext(context) as UpdateBuilder;
        }

        public static Task<int> UpdateAsync<T>(this IDbContext context, T model)
        {
            throw new NotImplementedException();
        }

        public static IDeleteBuilder DeleteFrom(this IDbContext context, string tableName)
        {
            return new DeleteBuilder(tableName)
                .WithContext(context) as DeleteBuilder;
        }

        public static Task<int> DeleteAsync<T>(this IDbContext context, T model)
        {
            throw new NotImplementedException();
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