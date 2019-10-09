namespace Normal
{
    internal class DeleteBuilder : StatementBuilder, IDeleteBuilder
    {
        public DeleteBuilder(string tableName)
        {
            AddLine("DELETE FROM", tableName);
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