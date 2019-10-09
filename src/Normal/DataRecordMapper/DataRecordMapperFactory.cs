using System;
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
        public IDataRecordMapper CreateMapper(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            var targetType = underlyingType ?? type;

            if (targetType.IsPrimitive || targetType.IsEnum || _primitiveExtensions.Any(primitiveExtension => primitiveExtension.IsAssignableFrom(targetType)))
            {
                return new PrimitiveDataRecordMapper();
            }
            else if (targetType == typeof(object))
            {
                return new DynamicDataRecordMapper();
            }
            else
            {
                return new ClassDataRecordMapper();
            }
        }
    }
}