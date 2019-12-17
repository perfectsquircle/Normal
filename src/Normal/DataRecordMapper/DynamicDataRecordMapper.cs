using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Normal
{
    internal class DynamicDataRecordMapper : IDataRecordMapper
    {
        private IEnumerable<string> _columnNames;

        public object MapDataRecord(IDataRecord dataRecord)
        {
            if (_columnNames == null)
            {
                _columnNames = GetColumnNames(dataRecord).Buffered();
            }

            IDictionary<string, object> instance = new ExpandoObject();
            foreach (var columnName in _columnNames)
            {
                instance[columnName] = dataRecord[columnName];
            }
            return instance;
        }

        private static IEnumerable<string> GetColumnNames(IDataRecord dataRecord)
        {
            for (var i = 0; i < dataRecord.FieldCount; i++)
            {
                yield return dataRecord.GetName(i);
            }
        }
    }
}