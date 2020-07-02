using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Normal
{
    internal class Table
    {
        private readonly Type _targetType;

        public Table(Type targetType)
        {
            _targetType = targetType;
        }

        public string Name
        {
            get
            {
                var tableNameAttribute = _targetType.GetCustomAttribute<TableAttribute>();
                var tableName = tableNameAttribute?.Name ?? _targetType.Name;
                return tableName;
            }
        }

        public IEnumerable<string> ColumnNames
        {
            get
            {
                return Member
                    .GetMembers(_targetType)
                    .Where(m => m.CanRead)
                    .Select(GetColumnName);
            }
        }

        public IDictionary<string, object> GetColumns<T>(T target)
        {
            return Member
                .GetMembers(_targetType)
                .Where(m => m.CanRead)
                .ToDictionary(m => GetColumnName(m), m => m.GetValue(target));
        }

        public string PrimaryKeyColumnName
        {
            get
            {
                var primaryKeyMember = GetPrimaryKeyMember();
                return GetColumnName(primaryKeyMember);
            }
        }

        public Tuple<string, object> GetPrimaryKey<T>(T target)
        {
            var primaryKeyMember = GetPrimaryKeyMember();
            return Tuple.Create(GetColumnName(primaryKeyMember), primaryKeyMember.GetValue(target));
        }

        public static string GetColumnName(Member m)
        {
            var columnNameAttribute = m.GetAttribute<ColumnAttribute>();
            return columnNameAttribute?.Name ?? m.Name;
        }

        private Member GetPrimaryKeyMember()
        {
            var primaryKeyMember = Member
                .GetMembers(_targetType)
                .FirstOrDefault(m =>
                {
                    return m.GetAttribute<PrimaryKeyAttribute>() != null;
                });

            if (primaryKeyMember == null)
            {
                primaryKeyMember = Member
                    .GetMembers(_targetType)
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