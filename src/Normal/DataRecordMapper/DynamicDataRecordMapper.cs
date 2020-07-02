using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace Normal
{
    internal class DynamicDataRecordMapper<dynamic> : IDataRecordMapper<dynamic>
    {
        private IEnumerable<string> _columnNames;

        public dynamic MapDataRecord(IDataRecord dataRecord)
        {
            if (_columnNames == null)
            {
                _columnNames = GetColumnNames(dataRecord).ToList();
            }

            IDictionary<string, object> instance = new ExpandoObject();
            foreach (var columnName in _columnNames)
            {
                instance[columnName] = dataRecord[columnName];
            }
            return (dynamic)instance;
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