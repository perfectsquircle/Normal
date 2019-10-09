using System.Linq;

namespace Normal
{
    internal class UpdateBuilder : StatementBuilder, IUpdateBuilder
    {
        private bool _setCalled = false;

        public UpdateBuilder(string tableName)
        {
            AddLine("UPDATE", tableName);
        }

        public IConditionBuilder<IUpdateBuilder> Set(string columnName)
        {
            var keyword = "SET";
            if (_setCalled)
            {
                keyword = ",";
            }
            _setCalled = true;
            return new ConditionBuilder<IUpdateBuilder>(this, keyword, columnName);
        }

        public IUpdateBuilder Set(object setBuilder)
        {
            var setPairs = ReflectionHelper.ToDictionary(setBuilder);
            var setList = setPairs.Select(setPair =>
            {
                var parameterName = RegisterParameter(setPair.Value);
                return $"{setPair.Key} = @{parameterName}";
            });
            return AddLine("SET", setList.ToArray());
        }

        public IConditionBuilder<IUpdateBuilder> Where(string columnName)
        {
            return new ConditionBuilder<IUpdateBuilder>(this, "WHERE", columnName);
        }

        public IConditionBuilder<IUpdateBuilder> And(string columnName)
        {
            return new ConditionBuilder<IUpdateBuilder>(this, "AND", columnName);
        }

        public IConditionBuilder<IUpdateBuilder> Or(string columnName)
        {
            return new ConditionBuilder<IUpdateBuilder>(this, "OR", columnName);
        }

        public new IUpdateBuilder AddLine(string keyword, params string[] columnNames)
        {
            return base.AddLine(keyword, columnNames) as IUpdateBuilder;
        }
    }
}