using System;
using System.Collections.Generic;
using System.Linq;

namespace Normal
{
    public partial class DbContext
    {
        private class DbContextBuilder : IDbContextBuilder
        {
            private readonly DbContext _dbContext;
            private readonly IList<DelegatingHandler> _delegatingHandlers;
            private readonly IDataRecordMapperFactory _dataRecordMapperFactory;

            public DbContextBuilder()
            {
                _dbContext = new DbContext();
                _delegatingHandlers = new List<DelegatingHandler>();
                _dataRecordMapperFactory = new DataRecordMapperFactory();
            }

            public IDbContextBuilder UseConnection(CreateConnection createConnection)
            {
                if (createConnection == null)
                {
                    throw new ArgumentNullException(nameof(createConnection));
                }
                _dbContext._createConnection = createConnection;
                return this;
            }

            public IDbContextBuilder UseDelegatingHandler(DelegatingHandler delegatingHandler)
            {
                if (delegatingHandler == null)
                {
                    throw new ArgumentNullException(nameof(delegatingHandler));
                }

                _delegatingHandlers.Add(delegatingHandler);
                return this;
            }

            public IDbContextBuilder UseDataRecordMapper(Type type, IDataRecordMapper mapper)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                _dataRecordMapperFactory.UseCustomMapper(type, mapper);
                return this;
            }

            public IDbContextBuilder UseDataRecordMapper(Type type, MapDataRecord mapDataRecord)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }
                if (mapDataRecord == null)
                {
                    throw new ArgumentNullException(nameof(mapDataRecord));
                }
                _dataRecordMapperFactory.UseCustomMapper(type, new AdHocDataRecordMapper(mapDataRecord));
                return this;
            }

            public DbContext Build()
            {
                var handler = BuildHandler(_dbContext);
                _dbContext._handler = handler;
                return _dbContext;
            }

            private IHandler BuildHandler(DbContext context)
            {
                IHandler head = new BaseHandler(context, _dataRecordMapperFactory);

                // Connect all the delegating handlers in a chain
                foreach (var handler in _delegatingHandlers.Reverse())
                {
                    handler.InnerHandler = head;
                    head = handler;
                }

                return head;
            }
        }
    }
}