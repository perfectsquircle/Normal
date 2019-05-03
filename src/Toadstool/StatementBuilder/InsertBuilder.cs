using System;
using System.Linq;

namespace Toadstool
{
    public class InsertBuilder : StatementBuilder
    {
        internal InsertBuilder(string tableName, string[] columnNames = null, IDbContext context = null)
        {
            if (columnNames != null)
            {
                AddLine($"INSERT INTO {tableName} ({string.Join(", ", columnNames)})");
            }
            else
            {
                AddLine($"INSERT INTO {tableName}");
            }
            this._context = context;
        }

        public InsertBuilder Values(params object[][] valueRows)
        {
            var valueRowStrings = valueRows.Select(GetValueRowParameterString);
            var values = string.Join(",\n", valueRowStrings);
            AddLine("VALUES", values);
            return this;
        }

        public InsertBuilder Values(params object[] valueRow)
        {
            AddLine("VALUES", GetValueRowParameterString(valueRow));
            return this;
        }

        public string GetValueRowParameterString(object[] valueRow)
        {
            var parameterStrings = valueRow.Select(valueRowValue =>
            {
                string parameterName = RegisterParameter(valueRowValue);
                return $"@{parameterName}";
            }).ToArray();
            return $"({string.Join(", ", parameterStrings)})";
        }

        public InsertBuilder Select(Func<SelectBuilder> select)
        {
            var selectBuilder = select.Invoke();
            AddLine($"({selectBuilder.Build()})");
            return this;
        }
    }
}