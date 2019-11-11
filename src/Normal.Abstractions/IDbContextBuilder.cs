using System;
using System.Data;

namespace Normal
{
    public interface IDbContextBuilder
    {
        IDbContextBuilder UseConnection(CreateConnection createConnection);
        IDbContextBuilder UseDelegatingHandler(DelegatingHandler delegatingHandler);
        IDbContextBuilder UseDataRecordMapper(Type type, IDataRecordMapper mapper);
        IDbContextBuilder UseDataRecordMapper(Type type, MapDataRecord mapDataRecord);
    }

    public delegate IDbConnection CreateConnection();
    public delegate object MapDataRecord(IDataRecord dataRecord);
}