using System;
using System.Collections.Generic;

namespace Toadstool
{
    public class SelectBuilder : IBuildableQuery
    {
        private IList<string> _stuff = new List<string>();
        public IDictionary<string, object> Parameters { get; }
        private readonly IDbContext _context;

        internal SelectBuilder(string[] selectList, IDbContext context = null)
        {
            Parameters = new Dictionary<string, object>();
            AddStuff("SELECT", selectList);
            this._context = context;
        }

        public SelectBuilder From(params string[] fromList)
        {
            return AddStuff("FROM", fromList);
        }

        public SelectBuilder Join(string joinList)
        {
            return AddStuff("JOIN", joinList);
        }

        public SelectBuilder LeftJoin(string joinList)
        {
            return AddStuff("LEFT JOIN", joinList);
        }

        public ConditionBuilder Where(string columnName)
        {
            return new ConditionBuilder(this, "WHERE", columnName);
        }

        public ConditionBuilder And(string columnName)
        {
            return new ConditionBuilder(this, "AND", columnName);
        }

        public ConditionBuilder Or(string columnName)
        {
            return new ConditionBuilder(this, "AND", columnName);
        }

        public SelectBuilder GroupBy(params string[] groupingElements)
        {
            return AddStuff("GROUP BY", groupingElements);
        }

        public SelectBuilder Having(string having)
        {
            return AddStuff("HAVING", having);
        }

        public SelectBuilder OrderBy(string orderBy)
        {
            return AddStuff("ORDER BY", orderBy);
        }

        public SelectBuilder Limit(int limit)
        {
            return AddStuff("LIMIT", limit.ToString());
        }

        public SelectBuilder Offset(int offset)
        {
            return AddStuff("OFFSET", offset.ToString());
        }

        public string Build()
        {
            return string.Join("\n", _stuff);
        }

        public IDbCommandBuilder Query()
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context.Query(this);
        }

        internal SelectBuilder AddStuff(string keyword, params string[] stuff)
        {
            var stuffString = string.Join(", ", stuff);
            _stuff.Add($"{keyword} {stuffString}");
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