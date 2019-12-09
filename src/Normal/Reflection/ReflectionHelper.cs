using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastMember;

namespace Normal
{
    internal static class ReflectionHelper
    {
        public static IDictionary<string, object> ToDictionary(object target)
        {
            var typeAccessor = TypeAccessor.Create(target.GetType());
            return typeAccessor
                .GetMembers()
                .Where(m => m.CanRead)
                .ToDictionary(m => GetColumnName(m), m => typeAccessor[target, m.Name]);
        }

        public static ConstructorInfo GetConstructor(Type connectionType, object[] arguments)
        {
            var argumentTypes = arguments.Select(a => a.GetType()).ToArray();
            var constructor = connectionType.GetConstructor(argumentTypes); ;
            if (constructor == null)
            {
                var argumentTypeStrings = string.Join(",", argumentTypes.Select(t => t.ToString()));
                throw new NotSupportedException($"No constructor found: {connectionType}({argumentTypeStrings})");
            }

            return constructor;
        }

        public static string GetTableName(Type targetType)
        {
            var tableNameAttribute = targetType.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
            var tableName = tableNameAttribute?.Name ?? targetType.Name;
            return tableName;
        }

        public static IEnumerable<string> GetColumnNames(Type targetType)
        {
            var typeAccessor = TypeAccessor.Create(targetType);
            return typeAccessor
                .GetMembers()
                .Select(GetColumnName);
        }

        public static string GetColumnName(Member m)
        {
            var columnNameAttribute = m.GetAttribute(typeof(ColumnAttribute), false) as ColumnAttribute;
            return columnNameAttribute?.Name ?? m.Name;
        }

        public static string GetPrimaryKeyColumnName(Type targetType)
        {
            var primaryKeyMember = GetPrimaryKeyMember(TypeAccessor.Create(targetType));
            return GetColumnName(primaryKeyMember);
        }

        public static (string, object) GetPrimaryKey<T>(T target)
        {
            var typeAccessor = TypeAccessor.Create(typeof(T));
            var primaryKeyMember = GetPrimaryKeyMember(typeAccessor);
            return (GetColumnName(primaryKeyMember), typeAccessor[target, primaryKeyMember.Name]);
        }

        private static Member GetPrimaryKeyMember(TypeAccessor typeAccessor)
        {
            var primaryKeyMember = typeAccessor
                .GetMembers()
                .FirstOrDefault(m =>
                {
                    return m.GetAttribute(typeof(PrimaryKeyAttribute), false) != null;
                });

            if (primaryKeyMember == null)
            {
                primaryKeyMember = typeAccessor
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