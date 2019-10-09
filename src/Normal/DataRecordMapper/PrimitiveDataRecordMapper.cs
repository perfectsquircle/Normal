using System;
using System.Data;

namespace Normal
{
    internal class PrimitiveDataRecordMapper : IDataRecordMapper
    {
        private PropertyMapper _propertyMapper;
        public T MapDataRecord<T>(IDataRecord dataRecord)
        {
            if (_propertyMapper == null)
            {
                _propertyMapper = new PropertyMapper()
                    .WithColumnIndex(0)
                    .WithPropertyType(typeof(T));
            }
            return (T)_propertyMapper.MapProperty(dataRecord);
        }
    }
}