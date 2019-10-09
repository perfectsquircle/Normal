using System;
using System.Data;

namespace Normal
{
    internal interface IDataRecordMapper
    {
        T MapDataRecord<T>(IDataRecord dataRecord);
    }
}