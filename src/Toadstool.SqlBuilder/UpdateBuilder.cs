using System;
using System.Collections.Generic;

namespace Toadstool
{
    public class UpdateBuilder : StatementBuilder
    {
        internal UpdateBuilder(string tableName, IDbContext context = null)
        {
            AddLine("UPDATE", tableName);
            this._context = context;
        }

        public UpdateBuilder Set(string expression)
        {
            return AddLine("SET", expression);
        }

        public UpdateBuilder Set(params object[] setPairs)
        {
            var setList = new List<string>();
            for (var i = 0; i < setPairs.Length; i += 2)
            {
                var left = Convert.ToString(setPairs[i]);
                var right = setPairs[i + 1];

                var parameterName = RegisterParameter(right);
                setList.Add($"{left} = @{parameterName}");
            }
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
    }
}