using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    public abstract class StatementBuilder : IStatementBuilder
    {
        protected IList<string> _lines = new List<string>();
        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
        protected IDbContext _context;

        public static SelectBuilder Select(params string[] selectList)
        {
            return new SelectBuilder(selectList);
        }

        public static InsertBuilder InsertInto(string tableName)
        {
            return new InsertBuilder(tableName);
        }

        public static UpdateBuilder Update(string tableName)
        {
            return new UpdateBuilder(tableName);
        }

        public static DeleteBuilder DeleteFrom(string tableName)
        {
            return new DeleteBuilder(tableName);
        }

        public string Build()
        {
            return string.Join("\n", _lines);
        }

        public IDbCommandBuilder ToCommand()
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context.Command(this);
        }

        public Task<int> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
             ToCommand().ExecuteAsync(cancellationToken);

        public Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) =>
            ToCommand().ExecuteAsync<T>(cancellationToken);


        public IStatementBuilder AddLine(string keyword, params string[] columnNames)
        {
            if (columnNames.Any())
            {
                _lines.Add($"{keyword} {string.Join(", ", columnNames)}");
            }
            else
            {
                _lines.Add($"{keyword}");
            }
            return this;
        }

        public string RegisterParameter(object value)
        {
            var parameterName = $"toadstool_{Parameters.Count + 1}";
            Parameters.Add(parameterName, value);
            return parameterName;
        }

        protected StatementBuilder WithContext(IDbContext context)
        {
            _context = context;
            return this;
        }
    }
}