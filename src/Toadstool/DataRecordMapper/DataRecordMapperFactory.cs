using System;
using System.Data;

namespace Toadstool
{
    internal class DataRecordMapperFactory : IDataRecordMapperFactory
    {
        public IDataRecordMapper CreateMapper(Type type)
        {
            if (type.IsPrimitive)
            {
                return new PrimitiveDataRecordMapper();
            }
            else if (type == typeof(object))
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