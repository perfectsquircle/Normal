using System.Linq;

namespace Toadstool
{
    public class InsertBuilder : StatementBuilder
    {
        private bool _valuesCalled = false;

        internal InsertBuilder(string tableName)
        {
            AddLine($"INSERT INTO {tableName}");
        }

        public InsertBuilder Columns(params string[] columnNames)
        {
            AddLine($"({string.Join(", ", columnNames)})");
            return this;
        }

        public InsertBuilder Values(params object[] valueRow)
        {
            var values = GetValueRowParameterString(valueRow);
            if (_valuesCalled)
            {
                AddLine($",{values}");
            }
            else
            {
                AddLine("VALUES");
                AddLine(values);
            }
            _valuesCalled = true;
            return this;
        }

        private string GetValueRowParameterString(object[] valueRow)
        {
            var parameterStrings = valueRow.Select(valueRowValue =>
            {
                var parameterName = RegisterParameter(valueRowValue);
                return $"@{parameterName}";
            }).ToArray();
            return $"({string.Join(", ", parameterStrings)})";
        }

        internal new InsertBuilder WithContext(IDbContext context)
        {
            base.WithContext(context);
            return this;
        }
    }
}