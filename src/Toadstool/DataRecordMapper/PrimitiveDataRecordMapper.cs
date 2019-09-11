using System;
using System.Data;

namespace Toadstool
{
    internal class PrimitiveDataRecordMapper : IDataRecordMapper
    {
        public T MapDataRecord<T>(IDataRecord dataRecord)
        {
            return (T)dataRecord[0];
        }
    }
}