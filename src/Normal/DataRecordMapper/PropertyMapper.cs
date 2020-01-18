using System;
using System.Data;
using System.Diagnostics;

namespace Normal
{
    internal class PropertyMapper
    {
        private int _columnIndex;
        private Type _columnType;
        private Type _propertyType;
        private Lazy<Func<IDataRecord, object>> _columnReader;
        public string PropertyName { get; set; }

        public PropertyMapper()
        {
            _columnReader = new Lazy<Func<IDataRecord, object>>(CreateColumnReader);
        }

        public object MapProperty(IDataRecord dataRecord)
        {
            Debug.Assert(dataRecord != null);
            if (dataRecord.IsDBNull(_columnIndex))
            {
                return null;
            }
            return _columnReader.Value(dataRecord);
        }

        public PropertyMapper WithColumnIndex(int columnIndex)
        {
            _columnIndex = columnIndex;
            return this;
        }

        public PropertyMapper WithColumnType(Type columnType)
        {
            _columnType = columnType;
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
            Debug.Assert(_columnType != null);
            Debug.Assert(_propertyType != null);

            if (_columnType == _propertyType || _propertyType.IsAssignableFrom(_columnType))
            {
                return (dataRecord) => dataRecord[_columnIndex];
            }

            if (_propertyType.IsEnum)
            {
                if (_columnType == typeof(string))
                {
                    return (dataRecord) => Enum.Parse(_propertyType, dataRecord.GetString(_columnIndex), true);
                }
                else
                {
                    return (dataRecord) => Enum.ToObject(_propertyType, dataRecord[_columnIndex]);
                }
            }

            return (dataRecord) => Convert.ChangeType(dataRecord[_columnIndex], _propertyType);
        }
    }
}