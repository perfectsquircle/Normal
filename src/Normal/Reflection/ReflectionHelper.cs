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

        public static string GetColumnName(Member m)
        {
            var columnNameAttribute = m.GetAttribute(typeof(ColumnAttribute), false) as ColumnAttribute;
            return columnNameAttribute?.Name ?? m.Name;
        }
    }
}