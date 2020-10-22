namespace Normal
{
    public interface IDeleteBuilder : IStatementBuilder, ICommandExecutor
    {
        IConditionBuilder<IDeleteBuilder> Where(string columnName);
        IConditionBuilder<IDeleteBuilder> And(string columnName);
        IConditionBuilder<IDeleteBuilder> Or(string columnName);
    }
}