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
    public static class IDatabaseExtensions
    {
        public static ISelectBuilder Select(this IDatabase database, params string[] selectList)
        {
            return new SelectBuilder(database).WithColumns(selectList);
        }

        public static async Task<IEnumerable<T>> SelectAsync<T>(this IDatabase database, CancellationToken cancellationToken = default)
        {
            return await database.Select<T>().ToEnumerableAsync<T>(cancellationToken);
        }

        public static async Task<IEnumerable<T>> SelectAsync<T>(
            this IDatabase database,
            Action<ISelectBuilder> builderCallback,
            CancellationToken cancellationToken = default)
        {
            var selectBuilder = database.Select<T>();
            builderCallback(selectBuilder);
            return await selectBuilder.ToEnumerableAsync<T>(cancellationToken);
        }

        public static async Task<T> SelectAsync<T>(
            this IDatabase database,
            object id,
            CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            return await database.Select<T>()
                .Where(table.PrimaryKey.Name).EqualTo(id)
                .FirstOrDefaultAsync<T>(cancellationToken);
        }

        public static IInsertBuilder InsertInto(this IDatabase database, string tableName)
        {
            return new InsertBuilder(database).WithTableName(tableName);
        }

        public static Task<T> InsertAsync<T>(
            this IDatabase database,
            T model,
            CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            var primaryKey = table.PrimaryKey;
            var columns = table.Columns;
            var columnsWithoutPrimaryKey = columns.Where(c => !c.IsPrimaryKey);
            var insertBuilder = database.InsertInto(table.Name)
                .Columns(columnsWithoutPrimaryKey.Select(c => c.Name).ToArray())
                .Values(columnsWithoutPrimaryKey.Select(c => c.GetValue(model)).ToArray()) as InsertBuilder;

            ISelectBuilder select;
            if (primaryKey.IsAutoIncrement)
            {
                var identityExpression = database.Variant switch
                {
                    Variant.SQLServer => "SCOPE_IDENTITY()",
                    Variant.PostgreSQL => "LASTVAL()",
                    Variant.MySQL => "LAST_INSERT_ID()",
                    _ => throw new NotSupportedException("Unknown database variant: " + database.Variant)
                };

                select = Select(database, columns.Select(c => c.Name).ToArray())
                    .From(table.Name)
                    .Where($"{primaryKey.Name} = {identityExpression}")
                    .End() as SelectBuilder;
            }
            else
            {
                select = Select(database, columns.Select(c => c.Name).ToArray())
                    .From(table.Name)
                    .Where($"{primaryKey.Name}").EqualTo(primaryKey.GetValue(model));
            }

            insertBuilder.AddLine(";");
            insertBuilder.AddLine(select.Build());


            return insertBuilder
                .SingleAsync<T>(cancellationToken);
        }

        public static IUpdateBuilder Update(this IDatabase database, string tableName)
        {
            return new UpdateBuilder(database).WithTableName(tableName);
        }

        public static Task<int> UpdateAsync<T>(
            this IDatabase database,
            T model,
            CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            var columns = table.Columns.ToList();
            var primaryKey = table.PrimaryKey;
            columns.Remove(primaryKey);
            return database.Update(table.Name)
                .Set(columns.ToDictionary(c => c.Name, c => c.GetValue(model)))
                .Where(primaryKey.Name).EqualTo(primaryKey.GetValue(model))
                .ExecuteNonQueryAsync(cancellationToken);
        }

        public static IDeleteBuilder DeleteFrom(this IDatabase database, string tableName)
        {
            return new DeleteBuilder(database).WithTableName(tableName);
        }

        public static Task<int> DeleteAsync<T>(this IDatabase database, T model, CancellationToken cancellationToken = default)
        {
            var table = new Table(typeof(T));
            var primaryKey = table.PrimaryKey;
            return database.DeleteFrom(table.Name)
                .Where(primaryKey.Name).EqualTo(primaryKey.GetValue(model))
                .ExecuteNonQueryAsync(cancellationToken);
        }

        public static IDbCommandBuilder CreateCommandFromFile(this IDatabase database, string fileName, Encoding encoding = default)
        {
            encoding = encoding ?? Encoding.Default;
            var file = File.ReadAllText(fileName, encoding);
            return database.CreateCommand(file);
        }

        public static IDbCommandBuilder CreateCommandFromResource(
            this IDatabase database,
            string resourceName,
            Assembly inputAssembly = default,
            Encoding encoding = default)
        {
            encoding ??= Encoding.Default;
            using var stream = FindResourceFromAssemblies(
                resourceName,
                inputAssembly,
                Assembly.GetCallingAssembly(),
                Assembly.GetEntryAssembly());
            if (stream == null)
            {
                throw new InvalidOperationException("Could not find resource named: " + resourceName);
            }
            using var reader = new StreamReader(stream, encoding);
            return database.CreateCommand(reader.ReadToEnd());
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


        // TODO: make this public, and return an ISelectBuilder with a Generic type?
        private static ISelectBuilder Select<T>(this IDatabase database)
        {
            var table = new Table(typeof(T));
            return database.Select(table.Columns.Select(c => c.Name).ToArray()).From(table.Name);
        }
    }
}