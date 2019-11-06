using System;
using System.Collections.Generic;
using System.Linq;

namespace Normal
{
    public class DbContextBuilder : IDbContextBuilder
    {
        private DbContext _dbContext;
        private IList<DelegatingHandler> _delegatingHandlers;

        public DbContextBuilder()
        {
            _dbContext = new DbContext();
            _delegatingHandlers = new List<DelegatingHandler>();
        }

        public IDbContextBuilder WithCreateConnection(CreateConnection createConnection)
        {
            if (createConnection == null)
            {
                throw new ArgumentNullException(nameof(createConnection));
            }
            _dbContext.CreateConnection = createConnection;
            return this;
        }

        public IDbContextBuilder WithDelegatingHandler(DelegatingHandler delegatingHandler)
        {
            if (delegatingHandler == null)
            {
                throw new ArgumentNullException(nameof(delegatingHandler));
            }

            _delegatingHandlers.Add(delegatingHandler);
            return this;
        }

        public IDbContextBuilder WithDataRecordMapper(Type type, IDataRecordMapper mapper)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _dbContext.DataRecordMapperFactory.WithCustomMapper(type, mapper);
            return this;
        }

        public IDbContextBuilder WithDataRecordMapper(Type type, MapDataRecord mapDataRecord)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (mapDataRecord == null)
            {
                throw new ArgumentNullException(nameof(mapDataRecord));
            }
            _dbContext.DataRecordMapperFactory.WithCustomMapper(type, new AdHocDataRecordMapper(mapDataRecord));
            return this;
        }

        public IDbContext Build()
        {
            var handler = BuildHandler();
            _dbContext.Handler = handler;
            return _dbContext;
        }

        private IHandler BuildHandler()
        {
            IHandler head = new BaseHandler();

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