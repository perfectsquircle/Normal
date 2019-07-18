namespace Toadstool
{
    public interface IDeleteBuilder : IStatementBuilder
    {
        IConditionBuilder<IDeleteBuilder> Where(string columnName);
        IConditionBuilder<IDeleteBuilder> And(string columnName);
        IConditionBuilder<IDeleteBuilder> Or(string columnName);
    }
}