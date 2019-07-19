using System;
using System.Data;

namespace Toadstool
{
    internal interface IDataRecordMapper
    {
        Func<IDataRecord, T> CompileMapper<T>(IDataRecord dataReader);
        Func<IDataRecord, dynamic> CompileMapper(IDataRecord dataReader);
    }
}