using System;
using System.Data;

namespace Normal
{
    internal class PrimitiveDataRecordMapper : IDataRecordMapper
    {
        private PropertyMapper _propertyMapper;
        private Type _targetType;

        public PrimitiveDataRecordMapper(Type targetType)
        {
            _targetType = targetType;
        }

        public object MapDataRecord(IDataRecord dataRecord)
        {
            if (_propertyMapper == null)
            {
                _propertyMapper = new PropertyMapper()
                    .WithColumnIndex(0)
                    .WithColumnType(dataRecord.GetFieldType(0))
                    .WithPropertyType(_targetType);
            }
            return _propertyMapper.MapProperty(dataRecord);
        }
    }
}