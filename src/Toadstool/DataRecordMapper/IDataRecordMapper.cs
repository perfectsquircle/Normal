using System;
using System.Data;

namespace Toadstool
{
    internal interface IDataRecordMapper
    {
        T MapDataRecord<T>(IDataRecord dataRecord);
    }
}