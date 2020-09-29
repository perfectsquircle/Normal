using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Normal
{
    public partial class Database
    {
        private class DatabaseBuilder : IDatabaseBuilder
        {
            public CreateConnection CreateConnection { get; private set; }
            private readonly IList<DelegatingHandler> _delegatingHandlers;
            private readonly IDataRecordMapperFactory _dataRecordMapperFactory;
            public Variant Variant { get; set; }

            public DatabaseBuilder()
            {
                _delegatingHandlers = new List<DelegatingHandler>();
                _dataRecordMapperFactory = new DataRecordMapperFactory();
            }

            public IDatabaseBuilder UseConnection<T>(params object[] arguments)
                where T : IDbConnection
            {
                var constructor = ReflectionHelper.GetConstructor(typeof(T), arguments);
                CreateConnection = () => (T)constructor.Invoke(arguments);
                Variant = DetermineVariant(typeof(T));
                return this;
            }

            public IDatabaseBuilder UseDelegatingHandler(DelegatingHandler delegatingHandler)
            {
                if (delegatingHandler == null)
                {
                    throw new ArgumentNullException(nameof(delegatingHandler));
                }

                _delegatingHandlers.Add(delegatingHandler);
                return this;
            }

            public IDatabaseBuilder UseDataRecordMapper<T>(IDataRecordMapper<T> mapper)
            {
                if (mapper == null)
                {
                    throw new ArgumentNullException(nameof(mapper));
                }

                _dataRecordMapperFactory.UseCustomMapper(mapper);
                return this;
            }

            public IDatabaseBuilder UseDataRecordMapper<T>(MapDataRecord<T> mapDataRecord)
            {
                if (mapDataRecord == null)
                {
                    throw new ArgumentNullException(nameof(mapDataRecord));
                }
                _dataRecordMapperFactory.UseCustomMapper(new AdHocDataRecordMapper<T>(mapDataRecord));
                return this;
            }

            public IHandler BuildHandler(Database database)
            {
                IHandler head = new BaseHandler(database, _dataRecordMapperFactory);

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