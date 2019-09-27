using System;
using System.Data;
using FastMember;

namespace Toadstool
{
    internal class PropertyMapper
    {
        private int _columnIndex;
        private Type _propertyType;
        private Func<object> _columnReader;
        public string PropertyName { get; set; }

        public object MapProperty(IDataRecord dataRecord)
        {
            if (_columnReader == null)
            {
                _columnReader = CreateColumnReader(dataRecord);
            }
            return _columnReader?.Invoke();
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

        private Func<object> CreateColumnReader(IDataRecord dataRecord)
        {
            if (dataRecord.IsDBNull(_columnIndex))
            {
                return () => null;
            }
            if (_propertyType.IsAssignableFrom(typeof(string)))
            {
                return () => dataRecord.GetString(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(bool)))
            {
                return () => dataRecord.GetBoolean(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(byte)))
            {
                return () => dataRecord.GetByte(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(short)))
            {
                return () => dataRecord.GetInt16(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(int)))
            {
                return () => dataRecord.GetInt32(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(long)))
            {
                return () => dataRecord.GetInt64(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(float)))
            {
                return () => dataRecord.GetFloat(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(double)))
            {
                return () => dataRecord.GetDouble(_columnIndex);
            }
            if (_propertyType.IsAssignableFrom(typeof(DateTime)))
            {
                return () => dataRecord.GetDateTime(_columnIndex);
            }

            return () => dataRecord[_columnIndex];
        }
    }
}