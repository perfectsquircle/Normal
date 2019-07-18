namespace Toadstool
{
    public interface IInsertBuilder : IStatementBuilder
    {
        IInsertBuilder Columns(params string[] columnNames);
        IInsertBuilder Values(params object[] valueRow);
    }
}