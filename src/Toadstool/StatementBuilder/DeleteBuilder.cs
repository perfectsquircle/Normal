namespace Toadstool
{
    public class DeleteBuilder : StatementBuilder
    {
        internal DeleteBuilder(string tableName)
        {
            AddLine("DELETE FROM", tableName);
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

        internal new DeleteBuilder WithContext(IDbContext context)
        {
            base.WithContext(context);
            return this;
        }
    }
}