using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
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

        public Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().ToListAsync<T>(cancellationToken);
        public Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().FirstAsync<T>(cancellationToken);
        public Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().FirstOrDefaultAsync<T>(cancellationToken);
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().SingleAsync<T>(cancellationToken);
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().SingleOrDefaultAsync<T>(cancellationToken);

        public Task<IList<dynamic>> ToListAsync(CancellationToken cancellationToken = default) =>
            ToCommand().ToListAsync(cancellationToken);
        public Task<dynamic> FirstAsync(CancellationToken cancellationToken = default) =>
            ToCommand().FirstAsync(cancellationToken);
        public Task<dynamic> FirstOrDefaultAsync(CancellationToken cancellationToken = default) =>
            ToCommand().FirstOrDefaultAsync(cancellationToken);
        public Task<dynamic> SingleAsync(CancellationToken cancellationToken = default) =>
            ToCommand().SingleAsync(cancellationToken);
        public Task<dynamic> SingleOrDefaultAsync(CancellationToken cancellationToken = default) =>
            ToCommand().SingleOrDefaultAsync(cancellationToken);

        private new ISelectBuilder AddLine(string keyword, params string[] columnNames)
        {
            return base.AddLine(keyword, columnNames) as ISelectBuilder;
        }
    }
}