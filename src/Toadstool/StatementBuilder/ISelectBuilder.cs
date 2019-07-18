using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public interface ISelectBuilder : IStatementBuilder
    {
        ISelectBuilder From(params string[] fromList);
        ISelectBuilder Join(string tableName);
        ISelectBuilder LeftJoin(string tableName);
        ISelectBuilder On(string clause);
        IConditionBuilder<ISelectBuilder> Where(string columnName);
        IConditionBuilder<ISelectBuilder> And(string columnName);
        IConditionBuilder<ISelectBuilder> Or(string columnName);
        ISelectBuilder GroupBy(params string[] groupingElements);
        ISelectBuilder Having(string having);
        ISelectBuilder OrderBy(string orderBy);
        ISelectBuilder Limit(int limit);
        ISelectBuilder Offset(int offset);
        Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstAsync<T>(CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleAsync<T>(CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default);

        Task<IList<dynamic>> ToListAsync(CancellationToken cancellationToken = default);
        Task<dynamic> FirstAsync(CancellationToken cancellationToken = default);
        Task<dynamic> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<dynamic> SingleAsync(CancellationToken cancellationToken = default);
        Task<dynamic> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    }
}