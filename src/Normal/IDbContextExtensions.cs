using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Normal
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