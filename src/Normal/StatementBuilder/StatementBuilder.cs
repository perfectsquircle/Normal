using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Normal
{
    internal abstract class StatementBuilder : IStatementBuilder
    {
        private IList<string> _lines = new List<string>();
        private IDictionary<string, object> _parameters = new Dictionary<string, object>();
        protected IDatabase _database;

        public string Build()
        {
            return string.Join(Environment.NewLine, _lines);
        }

        public override string ToString()
        {
            return Build();
        }

        public Task<IEnumerable<T>> ToEnumerableAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().ToEnumerableAsync<T>(cancellationToken);
        public Task<IList<T>> ToListAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().ToListAsync<T>(cancellationToken);
        public Task<T> FirstAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().FirstAsync<T>(cancellationToken);
        public Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().FirstOrDefaultAsync<T>(cancellationToken);
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().SingleAsync<T>(cancellationToken);
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().SingleOrDefaultAsync<T>(cancellationToken);
        public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default) =>
             ToCommand().ExecuteNonQueryAsync(cancellationToken);
        public Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default) =>
            ToCommand().ExecuteScalarAsync<T>(cancellationToken);

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
            var parameterName = $"normal_{_parameters.Count + 1}";
            _parameters.Add(parameterName, value);
            return parameterName;
        }

        protected ICommandBuilder ToCommand()
        {
            if (_database == null)
            {
                throw new InvalidOperationException("No database to execute against.");
            }
            return _database
                .CreateCommand(Build())
                .WithParameters(_parameters);
        }
    }
}