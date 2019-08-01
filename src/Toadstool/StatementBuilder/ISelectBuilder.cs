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
    }
}