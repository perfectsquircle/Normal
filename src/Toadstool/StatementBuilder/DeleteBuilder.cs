namespace Toadstool
{
    public class DeleteBuilder : StatementBuilder
    {
        internal DeleteBuilder(string tableName, IDbContext context = null)
        {
            AddLine("DELETE FROM", tableName);
            this._context = context;
        }

        public ConditionBuilder<DeleteBuilder> Where(string columnName)
        {
            return new ConditionBuilder<DeleteBuilder>(this, "WHERE", columnName);
        }

        public ConditionBuilder<DeleteBuilder> And(string columnName)
        {
            return new ConditionBuilder<DeleteBuilder>(this, "AND", columnName);
        }

        public ConditionBuilder<DeleteBuilder> Or(string columnName)
        {
            return new ConditionBuilder<DeleteBuilder>(this, "OR", columnName);
        }
    }
}