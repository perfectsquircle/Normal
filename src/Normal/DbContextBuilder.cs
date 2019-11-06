using System;
using System.Collections.Generic;
using System.Linq;

namespace Normal
{
    public class DbContextBuilder : IDbContextBuilder
    {
        private CreateConnection _createConnection;
        private IList<DelegatingHandler> _delegatingHandlers;

        public DbContextBuilder()
        {
            _delegatingHandlers = new List<DelegatingHandler>();
        }

        public IDbContextBuilder WithCreateConnection(CreateConnection createConnection)
        {
            if (createConnection == null)
            {
                throw new ArgumentNullException(nameof(createConnection));
            }
            _createConnection = createConnection;
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

        public IDbContext Build()
        {
            var context = new DbContext();
            var handler = BuildHandler(context);

            return context
                .WithHandler(handler)
                .WithCreateConnection(_createConnection);
        }

        private IHandler BuildHandler(DbContext context)
        {
            IHandler head = new BaseHandler(context);

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