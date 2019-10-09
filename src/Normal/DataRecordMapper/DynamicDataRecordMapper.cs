using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Normal
{
    internal class DynamicDataRecordMapper : IDataRecordMapper
    {
        private IList<string> _columnNames;
        public T MapDataRecord<T>(IDataRecord dataRecord)
        {
            if (_columnNames == null)
            {
                _columnNames = GetColumnNames(dataRecord);
            }

            IDictionary<string, object> instance = new ExpandoObject();
            foreach (var columnName in _columnNames)
            {
                instance[columnName] = dataRecord[columnName];
            }
            return (T)instance;
        }

        private static IList<string> GetColumnNames(IDataRecord dataRecord)
        {
            var columnNames = new List<string>();
            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                columnNames.Add(dataRecord.GetName(i));
            }
            return columnNames;
        }
    }
}