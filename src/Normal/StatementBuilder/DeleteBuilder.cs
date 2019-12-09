namespace Normal
{
    internal class DeleteBuilder : StatementBuilder, IDeleteBuilder
    {
        public DeleteBuilder(IDbContext context)
        {
            _context = context;
        }

        public DeleteBuilder(string tableName)
        {
            WithTableName(tableName);
        }

        public IDeleteBuilder WithTableName(string tableName)
        {
            AddLine("DELETE FROM", tableName);
            return this;
        }

        public IConditionBuilder<IDeleteBuilder> Where(string columnName)
        {
            return new ConditionBuilder<IDeleteBuilder>(this, "WHERE", columnName);
        }

        public IConditionBuilder<IDeleteBuilder> And(string columnName)
        {
            return new ConditionBuilder<IDeleteBuilder>(this, "AND", columnName);
        }

        public IConditionBuilder<IDeleteBuilder> Or(string columnName)
        {
            return new ConditionBuilder<IDeleteBuilder>(this, "OR", columnName);
        }
    }
}