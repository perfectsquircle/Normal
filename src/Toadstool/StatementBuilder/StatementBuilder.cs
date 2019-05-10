using System;
using System.Collections.Generic;
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

        public static InsertBuilder InsertInto(string tableName, params string[] columnNames)
        {
            return new InsertBuilder(tableName, columnNames);
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

        public Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            // return _context.ToListAsync<T>(this);
            return _context
                .Command(this.Build())
                .WithParameters(this.Parameters)
                .ToListAsync<T>(cancellationToken);
        }

        public Task<int> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context
                .Command(this.Build())
                .WithParameters(this.Parameters)
                .ExecuteAsync(cancellationToken);
        }

        public Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context
                .Command(this.Build())
                .WithParameters(this.Parameters)
                .ExecuteAsync<T>(cancellationToken);
        }

        public IStatementBuilder AddLine(string keyword, params string[] columnNames)
        {
            var columnNameString = string.Join(", ", columnNames);
            _lines.Add($"{keyword} {columnNameString}");
            return this;
        }

        public string RegisterParameter(object value)
        {
            var parameterName = $"toadstool_parameter_{Parameters.Count + 1}";
            Parameters.Add(parameterName, value);
            return parameterName;
        }
    }
}