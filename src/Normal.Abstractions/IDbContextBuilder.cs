using System;
using System.Data;

namespace Normal
{
    public interface IDbContextBuilder
    {
        IDbContextBuilder WithCreateConnection(CreateConnection createConnection);
        IDbContextBuilder WithDelegatingHandler(DelegatingHandler delegatingHandler);
        IDbContextBuilder WithDataRecordMapper(Type type, IDataRecordMapper mapper);
        IDbContextBuilder WithDataRecordMapper(Type type, MapDataRecord mapDataRecord);
        IDbContext Build();
    }

    public delegate IDbConnection CreateConnection();
    public delegate object MapDataRecord(IDataRecord dataRecord);
}