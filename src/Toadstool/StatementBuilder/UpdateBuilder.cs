using System.Linq;

namespace Toadstool
{
    public class UpdateBuilder : StatementBuilder
    {
        private bool _setCalled = false;

        internal UpdateBuilder(string tableName)
        {
            AddLine("UPDATE", tableName);
        }

        public ConditionBuilder<UpdateBuilder> Set(string columnName)
        {
            string keyword = "SET";
            if (_setCalled)
            {
                keyword = ",";
            }
            _setCalled = true;
            return new ConditionBuilder<UpdateBuilder>(this, keyword, columnName);
        }

        public UpdateBuilder Set(object setBuilder)
        {
            var setPairs = ReflectionHelper.ToDictionary(setBuilder);
            var setList = setPairs.Select(setPair =>
            {
                var parameterName = RegisterParameter(setPair.Value);
                return $"{setPair.Key} = @{parameterName}";
            });
            return AddLine("SET", setList.ToArray());
        }

        public ConditionBuilder<UpdateBuilder> Where(string columnName)
        {
            return new ConditionBuilder<UpdateBuilder>(this, "WHERE", columnName);
        }

        public ConditionBuilder<UpdateBuilder> And(string columnName)
        {
            return new ConditionBuilder<UpdateBuilder>(this, "AND", columnName);
        }

        public ConditionBuilder<UpdateBuilder> Or(string columnName)
        {
            return new ConditionBuilder<UpdateBuilder>(this, "OR", columnName);
        }

        internal new UpdateBuilder AddLine(string keyword, params string[] columnNames)
        {
            return base.AddLine(keyword, columnNames) as UpdateBuilder;
        }

        internal new UpdateBuilder WithContext(IDbContext context)
        {
            base.WithContext(context);
            return this;
        }
    }
}