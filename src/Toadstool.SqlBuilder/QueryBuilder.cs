using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toadstool
{
    public abstract class QueryBuilder : IQueryBuilder
    {
        protected IList<string> _lines = new List<string>();
        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        protected IDbContext _context;

        public string Build()
        {
            return string.Join("\n", _lines);
        }

        public IDbCommandBuilder Query()
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context.Query(this);
        }

        public Task<IList<T>> AsListOf<T>()
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context.AsListOf<T>(this);
        }

        public Task<int> ExecuteAsync()
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context.ExecuteAsync(this);
        }

        public Task<object> ExecuteScalarAsync()
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context.ExecuteScalarAsync(this);
        }

        public IQueryBuilder AddLine(string keyword, params string[] columnNames)
        {
            var columnNameString = string.Join(", ", columnNames);
            _lines.Add($"{keyword} {columnNameString}");
            return this;
        }

        public string RegisterParameter(object value)
        {
            var parameterName = $"toadstool_parameter_{Parameters.Count + 1}";
            Parameters.Add(parameterName, value);
            return parameterName;
        }
    }
}