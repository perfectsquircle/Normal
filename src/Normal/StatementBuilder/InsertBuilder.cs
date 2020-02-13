using System.Linq;

namespace Normal
{
    internal class InsertBuilder : StatementBuilder, IInsertBuilder
    {
        private bool _valuesCalled = false;

        public InsertBuilder(IDatabase database)
        {
            _database = database;
        }

        public InsertBuilder(string tableName)
        {
            WithTableName(tableName);
        }

        public IInsertBuilder WithTableName(string tableName)
        {
            AddLine($"INSERT INTO {tableName}");
            return this;
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