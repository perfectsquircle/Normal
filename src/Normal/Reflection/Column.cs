
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Normal
{
    internal class Column : Member
    {
        public override string Name => GetAttribute<ColumnAttribute>()?.Name ?? base.Name;
        public bool IsPrimaryKey => GetAttribute<PrimaryKeyAttribute>() != null;
        public bool IsAutoIncrement => GetAttribute<PrimaryKeyAttribute>()?.IsAutoIncrement ?? false;
        public bool IsMapped => GetAttribute<NotMappedAttribute>() == null;

        public Column(MemberInfo memberInfo)
            : base(memberInfo)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Column &&
                   base.Equals(obj);
        }

        public static IEnumerable<Column> GetColumns(Type targetType)
        {
            return targetType
                .GetFields()
                .Select(f => new Column(f)).Concat(
                    targetType
                        .GetProperties()
                        .Select(p => new Column(p))
                );
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}