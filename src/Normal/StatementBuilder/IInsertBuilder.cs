namespace Normal
{
    public interface IInsertBuilder : IStatementBuilder, ICommandExecutor
    {
        IInsertBuilder Columns(params string[] columnNames);
        IInsertBuilder Values(params object[] valueRow);
        IInsertBuilder Returning(params string[] columnNames);
    }
}