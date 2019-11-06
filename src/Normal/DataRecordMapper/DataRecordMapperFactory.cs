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

        private readonly IDictionary<Type, IDataRecordMapper> _customMappers;

        public DataRecordMapperFactory()
        {
            _customMappers = new Dictionary<Type, IDataRecordMapper>();
        }

        public IDataRecordMapper CreateMapper(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            var targetType = underlyingType ?? type;

            if (_customMappers.ContainsKey(targetType))
            {
                return _customMappers[targetType];
            }
            if (targetType.IsPrimitive || targetType.IsEnum || _primitiveExtensions.Any(primitiveExtension => primitiveExtension.IsAssignableFrom(targetType)))
            {
                return new PrimitiveDataRecordMapper(type);
            }
            else if (targetType == typeof(object))
            {
                return new DynamicDataRecordMapper();
            }
            else
            {
                return new ClassDataRecordMapper(type);
            }
        }

        public DataRecordMapperFactory WithCustomMapper(Type type, IDataRecordMapper mapper)
        {
            _customMappers.Add(type, mapper);
            return this;
        }
    }
}