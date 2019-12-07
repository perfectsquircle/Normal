using System.Collections.Generic;

namespace Normal
{
    public interface IUpdateBuilder : IStatementBuilder
    {
        IConditionBuilder<IUpdateBuilder> Set(string columnName);
        IUpdateBuilder Set(object setBuilder);
        IUpdateBuilder Set(IDictionary<string, object> setPairs);
        IConditionBuilder<IUpdateBuilder> Where(string columnName);
        IConditionBuilder<IUpdateBuilder> And(string columnName);
        IConditionBuilder<IUpdateBuilder> Or(string columnName);
    }
}