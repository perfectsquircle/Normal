using System.Data;

namespace Normal
{
    internal class AdHocDataRecordMapper : IDataRecordMapper
    {
        private MapDataRecord _mapDataRecord;

        public AdHocDataRecordMapper(MapDataRecord mapDataRecord)
        {
            _mapDataRecord = mapDataRecord;
        }

        public object MapDataRecord(IDataRecord dataRecord)
        {
            return _mapDataRecord.Invoke(dataRecord);
        }
    }
}