using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toadstool
{
    internal abstract class StatementBuilder : IStatementBuilder
    {
        private IList<string> _lines = new List<string>();
        private IDictionary<string, object> _parameters = new Dictionary<string, object>();
        private IDbContext _context;

        public string Build()
        {
            return string.Join("\n", _lines);
        }

        public Task<int> ExecuteAsync(CancellationToken cancellationToken = default) =>
             ToCommand().ExecuteAsync(cancellationToken);

        public Task<T> ExecuteAsync<T>(CancellationToken cancellationToken = default) =>
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
            var parameterName = $"toadstool_{_parameters.Count + 1}";
            _parameters.Add(parameterName, value);
            return parameterName;
        }

        public StatementBuilder WithContext(IDbContext context)
        {
            _context = context;
            return this;
        }

        protected IDbCommandBuilder ToCommand()
        {
            if (_context == null)
            {
                throw new NotSupportedException("No context to execute against.");
            }
            return _context
                .Command(this.Build())
                .WithParameters(this._parameters);
        }
    }
}