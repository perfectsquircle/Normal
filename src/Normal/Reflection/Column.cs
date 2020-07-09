
using System.Collections.Generic;

namespace Normal
{
    internal class Column
    {
        private readonly Member _member;
        public string Name => _member.GetAttribute<ColumnAttribute>()?.Name ?? _member.Name;
        public bool IsPrimaryKey => _member.GetAttribute<PrimaryKeyAttribute>() != null;
        public bool IsAutoIncrement => _member.GetAttribute<PrimaryKeyAttribute>()?.IsAutoIncrement ?? false;

        public Column(Member member)
        {
            _member = member;
        }

        public object GetValue(object target)
        {
            return _member.GetValue(target);
        }

        public override bool Equals(object obj)
        {
            return obj is Column column &&
                   EqualityComparer<Member>.Default.Equals(_member, column._member);
        }
    }
}