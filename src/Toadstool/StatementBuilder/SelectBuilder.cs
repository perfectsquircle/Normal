using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public class SelectBuilder : StatementBuilder
    {
        internal SelectBuilder(string[] selectList)
        {
            AddLine("SELECT", selectList);
        }

        public SelectBuilder From(params string[] fromList)
        {
            return AddLine("FROM", fromList);
        }

        public SelectBuilder Join(string tableName)
        {
            return AddLine("JOIN", tableName);
        }

        public SelectBuilder LeftJoin(string tableName)
        {
            return AddLine("LEFT JOIN", tableName);
        }

        public SelectBuilder On(string clause)
        {
            return AddLine("ON", clause);
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

        public Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().ToListAsync<T>(cancellationToken);
        public Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().FirstAsync<T>(cancellationToken);
        public Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().FirstOrDefaultAsync<T>(cancellationToken);
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().SingleAsync<T>(cancellationToken);
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().SingleOrDefaultAsync<T>(cancellationToken);

        public Task<List<dynamic>> ToListAsync(CancellationToken cancellationToken = default) =>
            ToCommand().ToListAsync(cancellationToken);
        public Task<dynamic> FirstAsync(CancellationToken cancellationToken = default) =>
            ToCommand().FirstAsync(cancellationToken);
        public Task<dynamic> FirstOrDefaultAsync(CancellationToken cancellationToken = default) =>
            ToCommand().FirstOrDefaultAsync(cancellationToken);
        public Task<dynamic> SingleAsync(CancellationToken cancellationToken = default) =>
            ToCommand().SingleAsync(cancellationToken);
        public Task<dynamic> SingleOrDefaultAsync(CancellationToken cancellationToken = default) =>
            ToCommand().SingleOrDefaultAsync(cancellationToken);

        internal new SelectBuilder AddLine(string keyword, params string[] columnNames)
        {
            return base.AddLine(keyword, columnNames) as SelectBuilder;
        }

        internal new SelectBuilder WithContext(IDbContext context)
        {
            base.WithContext(context);
            return this;
        }
    }
}