using System.Linq;

namespace Toadstool
{
    internal class InsertBuilder : StatementBuilder, IInsertBuilder
    {
        private bool _valuesCalled = false;

        public InsertBuilder(string tableName)
        {
            AddLine($"INSERT INTO {tableName}");
        }

        public IInsertBuilder Columns(params string[] columnNames)
        {
            AddLine($"({string.Join(", ", columnNames)})");
            return this;
        }

        public IInsertBuilder Values(params object[] valueRow)
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
    }
}