using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FastMember;

namespace Toadstool
{
    internal class ClassDataRecordMapper : IDataRecordMapper
    {
        private IDictionary<string, string> _columnToPropertyMap;
        private TypeAccessor _typeAccessor;

        public T MapDataRecord<T>(IDataRecord dataRecord)
        {
            if (_typeAccessor == null)
            {
                _typeAccessor = TypeAccessor.Create(typeof(T));
            }
            if (_columnToPropertyMap == null)
            {
                _columnToPropertyMap = CreateColumnToPropertyMap(dataRecord, _typeAccessor);
            }

            var instance = _typeAccessor.CreateNew();
            foreach (var columnToProperty in _columnToPropertyMap)
            {
                var fieldValue = dataRecord[columnToProperty.Key];
                if (fieldValue == DBNull.Value)
                {
                    continue;
                }
                _typeAccessor[instance, columnToProperty.Value] = fieldValue;
            }
            return (T)instance;
        }

        private static IEnumerable<string> GetVariants(string columnName)
        {
            yield return columnName;
            yield return columnName.ToLowerInvariant();
            yield return columnName.Replace("_", string.Empty);
            yield return columnName.ToLowerInvariant().Replace("_", string.Empty);
        }

        private static IDictionary<string, string> CreateColumnToPropertyMap(IDataRecord dataRecord, TypeAccessor typeAccessor)
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
    }
}