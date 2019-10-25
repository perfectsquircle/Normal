using System;
using System.Data;

namespace Normal
{
    internal interface IDataRecordMapper
    {
        object MapDataRecord(IDataRecord dataRecord);
    }
}