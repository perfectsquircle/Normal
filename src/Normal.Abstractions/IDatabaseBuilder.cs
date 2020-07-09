using System;
using System.Data;

namespace Normal
{
    public interface IDatabaseBuilder
    {
        IDatabaseBuilder UseConnection<T>(params object[] arguments) where T : IDbConnection;
        IDatabaseBuilder UseDelegatingHandler(DelegatingHandler delegatingHandler);
        IDatabaseBuilder UseDataRecordMapper<T>(IDataRecordMapper<T> mapper);
        IDatabaseBuilder UseDataRecordMapper<T>(MapDataRecord<T> mapDataRecord);
    }

    public delegate IDbConnection CreateConnection();
    public delegate T MapDataRecord<T>(IDataRecord dataRecord);
}