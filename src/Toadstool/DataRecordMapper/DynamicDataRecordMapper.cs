using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Toadstool
{
    internal class DynamicDataRecordMapper : IDataRecordMapper
    {
        private IList<string> _columnNames;
        public T MapDataRecord<T>(IDataRecord dataRecord)
        {
            var columnNames = GetColumnNames(dataRecord);

            IDictionary<string, object> instance = new ExpandoObject();
            foreach (var columnName in columnNames)
            {
                instance[columnName] = dataRecord[columnName];
            }
            return (T)instance;
        }

        private IList<string> GetColumnNames(IDataRecord dataRecord)
        {
            if (_columnNames == null)
            {
                var columnNames = new List<string>();
                for (var i = 0; i < dataRecord.FieldCount; i++)
                {
                    columnNames.Add(dataRecord.GetName(i));
                }

                _columnNames = columnNames;
            }
            return _columnNames;
        }
    }
}