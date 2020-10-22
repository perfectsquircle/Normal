using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Normal
{
    public interface ISelectBuilder : IStatementBuilder, ICommandExecutor
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
    }

    public interface ISelectBuilder<T> : IStatementBuilder, ICommandExecutor<T>
    {
        ISelectBuilder<T> From(params string[] fromList);
        ISelectBuilder<T> Join(string tableName);
        ISelectBuilder<T> LeftJoin(string tableName);
        ISelectBuilder<T> On(string clause);
        IConditionBuilder<ISelectBuilder<T>> Where(string columnName);
        IConditionBuilder<ISelectBuilder<T>> And(string columnName);
        IConditionBuilder<ISelectBuilder<T>> Or(string columnName);
        ISelectBuilder<T> GroupBy(params string[] groupingElements);
        ISelectBuilder<T> Having(string having);
        ISelectBuilder<T> OrderBy(string orderBy);
        ISelectBuilder<T> Limit(int limit);
        ISelectBuilder<T> Offset(int offset);
        TaskAwaiter<IEnumerable<T>> GetAwaiter();
    }
}