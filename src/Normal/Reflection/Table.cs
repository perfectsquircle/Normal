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
        private readonly Type _targetType;
        private string _tableName;
        private Column _primaryKeyColumn;
        private IEnumerable<Column> _columns;

        private Table(Type targetType)
        {
            _targetType = targetType;
        }

        public string Name
        {
            get
            {
                lock (this)
                {
                    _tableName ??= _targetType.GetCustomAttribute<TableAttribute>()?.Name
                         ?? _targetType.Name;
                    return _tableName;
                }
            }
        }

        public IEnumerable<Column> Columns
        {
            get
            {
                lock (this)
                {
                    _columns ??= Column
                        .GetColumns(_targetType)
                        .Where(m => m.CanRead
                            && m.GetAttribute<NotMappedAttribute>() == null);
                    return _columns;
                }
            }
        }

        public Column PrimaryKey
        {
            get
            {
                lock (this)
                {
                    _primaryKeyColumn ??= GetPrimaryKey();
                    return _primaryKeyColumn;
                }
            }
        }

        internal static Table FromType(Type type)
        {
            if (_cache.ContainsKey(type))
            {
                return _cache[type];
            }
            return _cache[type] = new Table(type);
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