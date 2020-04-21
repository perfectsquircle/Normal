using System.Data;

namespace Normal
{
    internal class AdHocDataRecordMapper<T> : IDataRecordMapper<T>
    {
        private MapDataRecord<T> _mapDataRecord;

        public AdHocDataRecordMapper(MapDataRecord<T> mapDataRecord)
        {
            _mapDataRecord = mapDataRecord;
        }

        public T MapDataRecord(IDataRecord dataRecord)
        {
            return _mapDataRecord.Invoke(dataRecord);
        }
    }
}