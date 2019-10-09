using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal class SelectBuilder : StatementBuilder, ISelectBuilder
    {
        public SelectBuilder(params string[] selectList)
        {
            AddLine("SELECT", selectList);
        }

        public ISelectBuilder From(params string[] fromList)
        {
            return AddLine("FROM", fromList);
        }

        public ISelectBuilder Join(string tableName)
        {
            return AddLine("JOIN", tableName);
        }

        public ISelectBuilder LeftJoin(string tableName)
        {
            return AddLine("LEFT JOIN", tableName);
        }

        public ISelectBuilder On(string clause)
        {
            return AddLine("ON", clause);
        }

        public IConditionBuilder<ISelectBuilder> Where(string columnName)
        {
            return new ConditionBuilder<ISelectBuilder>(this, "WHERE", columnName);
        }

        public IConditionBuilder<ISelectBuilder> And(string columnName)
        {
            return new ConditionBuilder<ISelectBuilder>(this, "AND", columnName);
        }

        public IConditionBuilder<ISelectBuilder> Or(string columnName)
        {
            return new ConditionBuilder<ISelectBuilder>(this, "OR", columnName);
        }

        public ISelectBuilder GroupBy(params string[] groupingElements)
        {
            return AddLine("GROUP BY", groupingElements);
        }

        public ISelectBuilder Having(string having)
        {
            return AddLine("HAVING", having);
        }

        public ISelectBuilder OrderBy(string orderBy)
        {
            return AddLine("ORDER BY", orderBy);
        }

        public ISelectBuilder Limit(int limit)
        {
            return AddLine("LIMIT", limit.ToString());
        }

        public ISelectBuilder Offset(int offset)
        {
            return AddLine("OFFSET", offset.ToString());
        }

        private new ISelectBuilder AddLine(string keyword, params string[] columnNames)
        {
            return base.AddLine(keyword, columnNames) as ISelectBuilder;
        }
    }
}