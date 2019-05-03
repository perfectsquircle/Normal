using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Toadstool
{
    internal class DefaultDataRecordMapper : IDataRecordMapper
    {
        public virtual Func<IDataRecord, T> CompileMapper<T>(IDataRecord dataReader)
        {
            var map = GetColumnToPropertyMap<T>(dataReader);

            return (dataRecord) =>
            {
                var instance = CreateInstance<T>();
                foreach (var kvp in map)
                {
                    kvp.Value.SetValue(instance, dataReader[kvp.Key]);
                }
                return instance;
            };
        }

        public virtual IDictionary<string, PropertyInfo> GetColumnToPropertyMap<T>(IDataRecord dataRecord)
        {
            var map = new Dictionary<string, PropertyInfo>();
            var properties = ReflectionHelper.ToDictionaryOfProperties(typeof(T));
            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                if (dataRecord.IsDBNull(i))
                {
                    continue;
                }
                var columnName = dataRecord.GetName(i);
                var columnNameVariants = GetVariants(columnName);
                var propertyName = properties.Keys.FirstOrDefault(p =>
                {
                    var propertyVariants = GetVariants(p);
                    return propertyVariants.Intersect(columnNameVariants).Any();
                });
                if (propertyName == default(string))
                {
                    continue;
                }
                var property = properties[propertyName];
                if (property == null || !property.CanWrite)
                {
                    continue;
                }
                map.Add(columnName, property);
            }
            return map;
        }

        public virtual T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public virtual IEnumerable<string> GetVariants(string columnName)
        {
            yield return columnName;
            yield return columnName.ToLowerInvariant();
            yield return columnName.Replace("_", "");
            yield return columnName.ToLowerInvariant().Replace("_", "");
        }
    }
}