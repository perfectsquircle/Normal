using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Normal
{
    internal class Table
    {
        private static readonly IDictionary<Type, Table> _cache = new ConcurrentDictionary<Type, Table>();
        private readonly Lazy<string> _tableName;
        private readonly Lazy<Column> _primaryKeyColumn;
        private readonly Lazy<IEnumerable<Column>> _columns;

        private Table(Type targetType)
        {
            _tableName = new Lazy<string>(() => targetType.GetCustomAttribute<TableAttribute>()?.Name
                         ?? targetType.Name);
            _primaryKeyColumn = new Lazy<Column>(GetPrimaryKey);
            _columns = new Lazy<IEnumerable<Column>>(() => Column
                        .GetColumns(targetType)
                        .Where(m => m.CanRead
                            && m.GetAttribute<NotMappedAttribute>() == null));
        }

        public string Name
            => _tableName.Value;

        public Column PrimaryKey
            => _primaryKeyColumn.Value;

        public IEnumerable<Column> Columns
            => _columns.Value;

        internal static Table FromType(Type type)
        {
            lock (_cache)
            {
                if (_cache.ContainsKey(type))
                {
                    return _cache[type];
                }
                return _cache[type] = new Table(type);
            }
        }

        private Column GetPrimaryKey()
        {
            var primaryKey = Columns
                .FirstOrDefault(m => m.IsPrimaryKey);

            if (primaryKey != null) return primaryKey;

            primaryKey = Columns
                .FirstOrDefault(m =>
                    string.Equals(m.Name, "id", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(m.Name, $"{Name}Id", StringComparison.OrdinalIgnoreCase));

            if (primaryKey != null) return primaryKey;

            throw new NotSupportedException("Could not determine primary key member.");
        }
    }
}