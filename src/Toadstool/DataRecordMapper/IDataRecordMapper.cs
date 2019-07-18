using System;
using System.Data;

namespace Toadstool
{
    public interface IDataRecordMapper
    {
        Func<IDataRecord, T> CompileMapper<T>(IDataRecord dataReader);
        Func<IDataRecord, dynamic> CompileMapper(IDataRecord dataReader);
    }
}