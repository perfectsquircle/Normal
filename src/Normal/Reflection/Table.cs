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

        public IEnumerable<Column> Columns
        {
            get
            {
                return Member
                    .GetMembers(_targetType)
                    .Where(m => m.CanRead)
                    .Select(m => new Column(m));
            }
        }

        public Column PrimaryKey
        {
            get
            {
                var primaryKeyMember = GetPrimaryKeyMember();
                return new Column(primaryKeyMember);
            }
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