using System;
using System.Collections.Generic;

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

        internal IQueryBuilder AddLine(string keyword, params string[] columnNames)
        {
            var columnNameString = string.Join(", ", columnNames);
            _lines.Add($"{keyword} {columnNameString}");
            return this;
        }

        internal string RegisterParameter(object value)
        {
            var parameterName = $"toadstool_parameter_{Parameters.Count + 1}";
            Parameters.Add(parameterName, value);
            return parameterName;
        }
    }
}