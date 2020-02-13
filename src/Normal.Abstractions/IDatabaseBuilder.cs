using System;
using System.Data;

namespace Normal
{
    public interface IDatabaseBuilder
    {
        IDatabaseBuilder UseConnection(CreateConnection createConnection);
        IDatabaseBuilder UseConnection<T>(params object[] arguments) where T : IDbConnection;
        IDatabaseBuilder UseDelegatingHandler(DelegatingHandler delegatingHandler);
        IDatabaseBuilder UseDataRecordMapper(Type type, IDataRecordMapper mapper);
        IDatabaseBuilder UseDataRecordMapper(Type type, MapDataRecord mapDataRecord);
    }

    public delegate IDbConnection CreateConnection();
    public delegate object MapDataRecord(IDataRecord dataRecord);
}