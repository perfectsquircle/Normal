using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Toadstool
{
    internal class DefaultDataRecordDeserializer : IDataRecordDeserializer
    {
        public T Deserialize<T>(IDataRecord dataRecord)
        {
            T obj = CreateInstance<T>();
            var properties = ReflectionHelper.ToDictionaryOfProperties(obj);
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
                property.SetValue(obj, dataRecord[columnName]);
            }
            return obj;
        }

        public virtual T CreateInstance<T>()
        {
            T obj = default(T);
            obj = (T)Activator.CreateInstance<T>();
            return obj;
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