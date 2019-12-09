using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastMember;

namespace Normal
{
    internal class Table
    {
        private readonly Type _targetType;
        private readonly Lazy<TypeAccessor> _typeAccessor;
        private TypeAccessor TypeAccessor => _typeAccessor.Value;

        public Table(Type targetType)
        {
            _targetType = targetType;
            _typeAccessor = new Lazy<TypeAccessor>(() => TypeAccessor.Create(_targetType));
        }

        public string Name
        {
            get
            {
                var tableNameAttribute = _targetType.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
                var tableName = tableNameAttribute?.Name ?? _targetType.Name;
                return tableName;
            }
        }

        public IEnumerable<string> Columns
        {
            get
            {
                return TypeAccessor
                    .GetMembers()
                    .Where(m => m.CanRead)
                    .Select(GetColumnName);
            }
        }

        public IDictionary<string, object> GetColumns<T>(T target)
        {
            return TypeAccessor
                .GetMembers()
                .Where(m => m.CanRead)
                .ToDictionary(m => GetColumnName(m), m => TypeAccessor[target, m.Name]);
        }

        public string PrimaryKey
        {
            get
            {
                var primaryKeyMember = GetPrimaryKeyMember();
                return GetColumnName(primaryKeyMember);
            }
        }

        public (string, object) GetPrimaryKey<T>(T target)
        {
            var primaryKeyMember = GetPrimaryKeyMember();
            return (GetColumnName(primaryKeyMember), TypeAccessor[target, primaryKeyMember.Name]);
        }

        private static string GetColumnName(Member m)
        {
            return ReflectionHelper.GetColumnName(m);
        }

        private Member GetPrimaryKeyMember()
        {
            var primaryKeyMember = TypeAccessor
                .GetMembers()
                .FirstOrDefault(m =>
                {
                    return m.GetAttribute(typeof(PrimaryKeyAttribute), false) != null;
                });

            if (primaryKeyMember == null)
            {
                primaryKeyMember = TypeAccessor
                    .GetMembers()
                    .FirstOrDefault(m => m.Name.ToLowerInvariant() == "id");
                if (primaryKeyMember == null)
                {
                    throw new NotSupportedException("Could not determine primary key member.");
                }
            }

            return primaryKeyMember;
        }
    }
}