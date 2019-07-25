using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using FastMember;

namespace Toadstool
{
    internal class DefaultDataRecordMapper : IDataRecordMapper
    {
        public virtual Func<IDataRecord, T> CompileMapper<T>(IDataRecord dataReader)
        {
            var typeAccessor = TypeAccessor.Create(typeof(T));
            var columnToPropertyMap = GetColumnToPropertyMap(dataReader, typeAccessor);

            return (dataRecord) =>
            {
                var instance = typeAccessor.CreateNew();
                foreach (var columnToProperty in columnToPropertyMap)
                {
                    var fieldValue = dataRecord[columnToProperty.Key];
                    if (fieldValue == DBNull.Value)
                    {
                        continue;
                    }
                    typeAccessor[instance, columnToProperty.Value] = fieldValue;
                }
                return (T)instance;
            };
        }

        public Func<IDataRecord, dynamic> CompileMapper(IDataRecord dataReader)
        {
            var columnNames = new List<string>();
            for (var i = 0; i < dataReader.FieldCount; i++)
            {
                columnNames.Add(dataReader.GetName(i));
            }

            return (dataRecord) =>
            {
                IDictionary<string, object> instance = new ExpandoObject();
                foreach (var columnName in columnNames)
                {
                    instance[columnName] = dataRecord[columnName];
                }
                return instance;
            };
        }

        private static IDictionary<string, string> GetColumnToPropertyMap(IDataRecord dataRecord, TypeAccessor typeAccessor)
        {
            var map = new Dictionary<string, string>();
            var members = typeAccessor
                .GetMembers()
                .Where(m => m.CanWrite);

            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                var columnName = dataRecord.GetName(i);
                var columnNameVariants = GetVariants(columnName);
                var member = members.FirstOrDefault(m =>
                {
                    var propertyVariants = GetVariants(m.Name);
                    return propertyVariants.Intersect(columnNameVariants).Any();
                });
                if (member == default(Member))
                {
                    continue;
                }
                map.Add(columnName, member.Name);
            }
            return map;
        }

        private static IEnumerable<string> GetVariants(string columnName)
        {
            yield return columnName;
            yield return columnName.ToLowerInvariant();
            yield return columnName.Replace("_", string.Empty);
            yield return columnName.ToLowerInvariant().Replace("_", string.Empty);
        }
    }
}