using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class SelectBuilder : StatementBuilder
    {
        internal SelectBuilder(string[] selectList, IDbContext context = null)
        {
            AddLine("SELECT", selectList);
            this._context = context;
        }

        public SelectBuilder From(params string[] fromList)
        {
            return AddLine("FROM", fromList);
        }

        public SelectBuilder Join(string joinList)
        {
            return AddLine("JOIN", joinList);
        }

        public SelectBuilder LeftJoin(string joinList)
        {
            return AddLine("LEFT JOIN", joinList);
        }

        public ConditionBuilder<SelectBuilder> Where(string columnName)
        {
            return new ConditionBuilder<SelectBuilder>(this, "WHERE", columnName);
        }

        public ConditionBuilder<SelectBuilder> And(string columnName)
        {
            return new ConditionBuilder<SelectBuilder>(this, "AND", columnName);
        }

        public ConditionBuilder<SelectBuilder> Or(string columnName)
        {
            return new ConditionBuilder<SelectBuilder>(this, "OR", columnName);
        }

        public SelectBuilder GroupBy(params string[] groupingElements)
        {
            return AddLine("GROUP BY", groupingElements);
        }

        public SelectBuilder Having(string having)
        {
            return AddLine("HAVING", having);
        }

        public SelectBuilder OrderBy(string orderBy)
        {
            return AddLine("ORDER BY", orderBy);
        }

        public SelectBuilder Limit(int limit)
        {
            return AddLine("LIMIT", limit.ToString());
        }

        public SelectBuilder Offset(int offset)
        {
            return AddLine("OFFSET", offset.ToString());
        }

        public Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) =>
            ToCommand().ToListAsync<T>();

        public Task<T> FirstAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) =>
            ToCommand().FirstAsync<T>();
        public Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) =>
            ToCommand().FirstOrDefaultAsync<T>();
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) =>
            ToCommand().SingleAsync<T>();
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) =>
            ToCommand().SingleOrDefaultAsync<T>();

        internal new SelectBuilder AddLine(string keyword, params string[] columnNames)
        {
            return base.AddLine(keyword, columnNames) as SelectBuilder;
        }
    }
}