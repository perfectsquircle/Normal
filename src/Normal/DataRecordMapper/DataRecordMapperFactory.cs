using System;
using System.Collections.Generic;
using System.Linq;

namespace Normal
{
    internal class DataRecordMapperFactory : IDataRecordMapperFactory
    {
        private static readonly Type[] _primitiveExtensions = new Type[]
        {
            typeof(decimal),
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(byte[]),
            typeof(char[]),
        };

        private readonly IDictionary<Type, object> _customMappers;

        public DataRecordMapperFactory()
        {
            _customMappers = new Dictionary<Type, object>();
        }

        public IDataRecordMapper<T> CreateMapper<T>()
        {
            var type = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(type);
            var targetType = underlyingType ?? type;

            if (_customMappers.ContainsKey(targetType))
            {
                return (IDataRecordMapper<T>)_customMappers[targetType];
            }
            else if (targetType.IsPrimitive || targetType.IsEnum || _primitiveExtensions.Any(primitiveExtension => primitiveExtension.IsAssignableFrom(targetType)))
            {
                return new PrimitiveDataRecordMapper<T>();
            }
            else if (targetType == typeof(object))
            {
                return new DynamicDataRecordMapper<T>();
            }
            else
            {
                return new ClassDataRecordMapper<T>();
            }
        }

        public IDataRecordMapperFactory UseCustomMapper<T>(IDataRecordMapper<T> mapper)
        {
            _customMappers.Add(typeof(T), mapper);
            return this;
        }
    }
}