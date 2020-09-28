using System;
using System.Data;

namespace Normal
{
    internal class PrimitiveDataRecordMapper<T> : IDataRecordMapper<T>
    {
        private const int _columnIndex = 0;
        private Func<IDataRecord, T> _columnReader;

        public T MapDataRecord(IDataRecord dataRecord)
        {
            if (_columnReader == null)
            {
                _columnReader = MemberMatch.GetColumnReader<T>(dataRecord.GetFieldType(_columnIndex), _columnIndex);
            }
            return _columnReader(dataRecord);
        }
    }
}