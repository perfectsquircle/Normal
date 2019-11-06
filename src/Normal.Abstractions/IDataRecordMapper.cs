using System;
using System.Data;

namespace Normal
{
    public interface IDataRecordMapper
    {
        object MapDataRecord(IDataRecord dataRecord);
    }
}