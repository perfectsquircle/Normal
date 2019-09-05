using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FastMember;

namespace Toadstool
{
    internal class ClassDataRecordMapper : IDataRecordMapper
    {
        private IDictionary<string, string> _map;
        private TypeAccessor _typeAccessor;

        public T MapDataRecord<T>(IDataRecord dataRecord)
        {
            var typeAccessor = GetTypeAccessor(typeof(T));
            var columnToPropertyMap = GetColumnToPropertyMap(dataRecord, typeAccessor);

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
        }

        private static IEnumerable<string> GetVariants(string columnName)
        {
            yield return columnName;
            yield return columnName.ToLowerInvariant();
            yield return columnName.Replace("_", string.Empty);
            yield return columnName.ToLowerInvariant().Replace("_", string.Empty);
        }

        private TypeAccessor GetTypeAccessor(Type type)
        {
            if (_typeAccessor == null)
            {
                _typeAccessor = TypeAccessor.Create(type);
            }
            return _typeAccessor;
        }

        private IDictionary<string, string> GetColumnToPropertyMap(IDataRecord dataRecord, TypeAccessor typeAccessor)
        {
            if (_map == null)
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
                _map = map;
            }
            return _map;
        }
    }
}