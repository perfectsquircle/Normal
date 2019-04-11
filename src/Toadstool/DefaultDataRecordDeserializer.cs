using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Toadstool
{
    internal class DefaultDataRecordDeserializer : IDataRecordDeserializer
    {
        private readonly ConcurrentDictionary<string, IDictionary<string, PropertyInfo>> _runtimePropertyCache;

        public DefaultDataRecordDeserializer()
        {
            _runtimePropertyCache = new ConcurrentDictionary<string, IDictionary<string, PropertyInfo>>();
        }

        public T Deserialize<T>(IDataRecord dataRecord)
        {
            T obj = default(T);
            obj = Activator.CreateInstance<T>();
            var properties = GetRuntimeProperties(obj);
            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                if (dataRecord.IsDBNull(i))
                {
                    continue;
                }
                var columnName = dataRecord.GetName(i);
                if (!properties.ContainsKey(columnName.ToLowerInvariant()))
                {
                    continue;
                }
                var property = properties[columnName.ToLowerInvariant()];
                if (property == null || !property.CanWrite)
                {
                    continue;
                }
                property.SetValue(obj, dataRecord[columnName]);
            }
            return obj;
        }

        private IDictionary<string, PropertyInfo> GetRuntimeProperties<T>(T obj)
        {
            var type = obj.GetType();
            var key = type.FullName;
            if (!_runtimePropertyCache.ContainsKey(key))
            {
                _runtimePropertyCache[key] = type
                    .GetRuntimeProperties()
                    .ToDictionary(p => p.Name.ToLowerInvariant());
            }
            return _runtimePropertyCache[key];
        }
    }
}