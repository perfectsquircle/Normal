using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal class SelectBuilder : StatementBuilder, ISelectBuilder
    {
        public SelectBuilder(IDatabase database)
        {
            _database = database;
        }

        public SelectBuilder(params string[] selectList)
        {
            WithColumns(selectList);
        }

        public ISelectBuilder WithColumns(params string[] selectList)
        {
            return AddLine("SELECT", selectList);
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

    internal partial class SelectBuilder<T> : StatementBuilder, ISelectBuilder<T>
    {
        public SelectBuilder(IDatabase database)
        {
            _database = database;
        }

        public SelectBuilder(params string[] selectList)
        {
            WithColumns(selectList);
        }

        public ISelectBuilder<T> WithColumns(params string[] selectList)
        {
            return AddLine("SELECT", selectList);
        }

        public ISelectBuilder<T> From(params string[] fromList)
        {
            return AddLine("FROM", fromList);
        }

        public ISelectBuilder<T> Join(string tableName)
        {
            return AddLine("JOIN", tableName);
        }

        public ISelectBuilder<T> LeftJoin(string tableName)
        {
            return AddLine("LEFT JOIN", tableName);
        }

        public ISelectBuilder<T> On(string clause)
        {
            return AddLine("ON", clause);
        }

        public IConditionBuilder<ISelectBuilder<T>> Where(string columnName)
        {
            return new ConditionBuilder<ISelectBuilder<T>>(this, "WHERE", columnName);
        }

        public IConditionBuilder<ISelectBuilder<T>> And(string columnName)
        {
            return new ConditionBuilder<ISelectBuilder<T>>(this, "AND", columnName);
        }

        public IConditionBuilder<ISelectBuilder<T>> Or(string columnName)
        {
            return new ConditionBuilder<ISelectBuilder<T>>(this, "OR", columnName);
        }

        public ISelectBuilder<T> GroupBy(params string[] groupingElements)
        {
            return AddLine("GROUP BY", groupingElements);
        }

        public ISelectBuilder<T> Having(string having)
        {
            return AddLine("HAVING", having);
        }

        public ISelectBuilder<T> OrderBy(string orderBy)
        {
            return AddLine("ORDER BY", orderBy);
        }

        public ISelectBuilder<T> Limit(int limit)
        {
            return AddLine("LIMIT", limit.ToString());
        }

        public ISelectBuilder<T> Offset(int offset)
        {
            return AddLine("OFFSET", offset.ToString());
        }

        public TaskAwaiter<IEnumerable<T>> GetAwaiter()
        {
            return ToEnumerableAsync<T>().GetAwaiter();
        }

        private new ISelectBuilder<T> AddLine(string keyword, params string[] columnNames)
        {
            return base.AddLine(keyword, columnNames) as ISelectBuilder<T>;
        }
    }

    internal partial class SelectBuilder<T> : ICommandExecutor<T>
    {
        public Task<T> ExecuteScalarAsync(CancellationToken cancellationToken = default)
            => ExecuteScalarAsync<T>(cancellationToken);
        public Task<T> FirstAsync(CancellationToken cancellationToken = default)
            => FirstAsync<T>(cancellationToken);
        public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
            => FirstOrDefaultAsync<T>(cancellationToken);
        public Task<T> SingleAsync(CancellationToken cancellationToken = default)
            => SingleAsync<T>(cancellationToken);
        public Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
            => SingleOrDefaultAsync<T>(cancellationToken);
        public Task<IEnumerable<T>> ToEnumerableAsync(CancellationToken cancellationToken = default)
            => ToEnumerableAsync<T>(cancellationToken);
        public Task<IList<T>> ToListAsync(CancellationToken cancellationToken = default)
            => ToListAsync<T>(cancellationToken);
    }
}