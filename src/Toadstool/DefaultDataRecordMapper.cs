using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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
                    var value = dataReader[kvp.Key];
                    if (value == DBNull.Value)
                    {
                        continue;
                    }
                    kvp.Value.SetValue(instance, value);
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

        public virtual Func<IDataRecord, dynamic> CompileMapper(IDataRecord dataReader)
        {
            var columnNames = new List<string>();
            for (var i = 0; i < dataReader.FieldCount; i++)
            {
                columnNames.Add(dataReader.GetName(i));
            }

            return (dataRecord) =>
            {
                var instance = new ExpandoObject();
                foreach (var columnName in columnNames)
                {
                    ((IDictionary<string, object>)instance)[columnName] = dataRecord[columnName];
                }
                return instance;
            };
        }
    }
}