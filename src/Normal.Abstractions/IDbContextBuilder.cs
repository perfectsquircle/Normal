using System;
using System.Data;

namespace Normal
{
    public interface IDbContextBuilder
    {
        IDbContextBuilder WithCreateConnection(CreateConnection createConnection);
        IDbContextBuilder WithDelegatingHandler(DelegatingHandler delegatingHandler);
        IDbContext Build();
    }

    public delegate IDbConnection CreateConnection();
}