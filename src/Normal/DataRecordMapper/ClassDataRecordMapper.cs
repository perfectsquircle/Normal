using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static System.Linq.Expressions.Expression;

namespace Normal
{
    internal class ClassDataRecordMapper<T> : IDataRecordMapper<T>
    {
        private Type _targetType = typeof(T);
        Func<IDataRecord, T> _constructor;


        public T MapDataRecord(IDataRecord dataRecord)
        {
            if (_constructor == null)
            {
                var matches = FindMatches(dataRecord).ToList();
                _constructor = CreateContructor(matches);
            }

            return _constructor(dataRecord);
        }

        private static IEnumerable<string> GetVariants(string columnName)
        {
            yield return columnName;
            yield return columnName.ToLowerInvariant();
            yield return columnName.Replace("_", string.Empty);
            yield return columnName.ToLowerInvariant().Replace("_", string.Empty);
        }

        private static IEnumerable<MemberMatch> FindMatches(IDataRecord dataRecord)
        {
            var members = Member.GetMembers(typeof(T))
                .Where(m => m.CanWrite)
                .ToList();

            var fields = typeof(T).GetFields().Where(f => f.IsPublic);

            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                var columnName = dataRecord.GetName(i);
                var columnNameVariants = GetVariants(columnName);
                var member = members.FirstOrDefault(m =>
                {
                    var columnNameAttribute = m.GetAttribute<ColumnAttribute>(false);
                    if (columnNameAttribute != null)
                    {
                        return columnNameAttribute.Name == columnName;
                    }
                    var propertyVariants = GetVariants(m.Name);
                    return propertyVariants.Intersect(columnNameVariants).Any();
                });
                if (member == default)
                {
                    continue;
                }
                members.Remove(member);
                yield return new MemberMatch
                {
                    ColumnIndex = i,
                    ColumnType = dataRecord.GetFieldType(i),
                    Member = member,
                };
            }
        }

        private Func<IDataRecord, T> CreateContructor(IEnumerable<MemberMatch> memberMatches)
        {
            var dataRecord = Parameter(typeof(IDataRecord), "dataRecord");

            var propertyBindings = memberMatches.Select(m =>
            {
                var columnReader = m.Member.GetColumnReader(dataRecord, m.ColumnType, m.ColumnIndex);

                return Bind(
                    m.Member.MemberInfo,
                    columnReader);
            });

            var memberInitExpression =
                MemberInit(
                    New(typeof(T)),
                    propertyBindings.ToArray());

            var lambda = Lambda<Func<IDataRecord, T>>(memberInitExpression, false, dataRecord);

            return lambda.Compile();
        }

        private class MemberMatch
        {
            public int ColumnIndex { get; set; }
            public Type ColumnType { get; set; }
            public Member Member { get; set; }
        }
    }
}