using System;
using System.Data;
using FastMember;

namespace Normal
{
    internal class PropertyMapper
    {
        private int _columnIndex;
        private Type _propertyType;
        private Func<IDataRecord, object> _columnReader;
        public string PropertyName { get; set; }

        public object MapProperty(IDataRecord dataRecord)
        {
            if (_columnReader == null)
            {
                _columnReader = CreateColumnReader();
            }
            if (dataRecord.IsDBNull(_columnIndex))
            {
                return null;
            }
            return _columnReader?.Invoke(dataRecord);
        }

        public PropertyMapper WithColumnIndex(int columnIndex)
        {
            _columnIndex = columnIndex;
            return this;
        }

        public PropertyMapper WithPropertyName(string propertyName)
        {
            PropertyName = propertyName;
            return this;
        }

        public PropertyMapper WithPropertyType(Type propertyType)
        {
            _propertyType = propertyType;
            return this;
        }

        public Func<IDataRecord, object> CreateColumnReader()
        {
            if (_propertyType.IsAssignableFrom(typeof(string)))
            {
                return (dataRecord) => dataRecord.GetString(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(bool)))
            {
                return (dataRecord) => dataRecord.GetBoolean(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(byte)))
            {
                return (dataRecord) => dataRecord.GetByte(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(short)))
            {
                return (dataRecord) => dataRecord.GetInt16(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(int)))
            {
                return (dataRecord) => dataRecord.GetInt32(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(long)))
            {
                return (dataRecord) => dataRecord.GetInt64(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(float)))
            {
                return (dataRecord) => dataRecord.GetFloat(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(double)))
            {
                return (dataRecord) => dataRecord.GetDouble(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(DateTime)))
            {
                return (dataRecord) => dataRecord.GetDateTime(_columnIndex);
            }

            return (dataRecord) => dataRecord[_columnIndex];
        }
    }
}