using System.Collections.Generic;

namespace Toadstool
{
    public class SelectBuilder
    {
        // private readonly string[] _selectList;
        // private string[] _fromList;
        // private IList<string> _joinList = new List<string>();

        private IList<string> _stuff = new List<string>();

        internal SelectBuilder(string[] selectList)
        {
            // this._selectList = selectList;
            AddStuff("SELECT", selectList);
        }

        public SelectBuilder From(params string[] fromList)
        {
            // _fromList = from;
            return AddStuff("FROM", fromList);
        }

        public SelectBuilder Join(string joinList)
        {
            return AddStuff("JOIN", joinList);
        }

        public SelectBuilder Where(string where)
        {
            return AddStuff("WHERE", where);
        }

        public SelectBuilder And(string and)
        {
            return AddStuff("AND", and);
        }

        public SelectBuilder Or(string or)
        {
            return AddStuff("OR", or);
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

        public string Build()
        {
            return string.Join("\n", _stuff);
        }

        private SelectBuilder AddStuff(string keyword, params string[] stuff)
        {
            var stuffString = string.Join(", ", stuff);
            _stuff.Add($"{keyword} {stuffString}");
            return this;
        }
    }
}