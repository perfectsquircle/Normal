using System.Collections.Generic;

namespace Toadstool
{
    public interface IStatementBuilder
    {
        string Build();
        IDictionary<string, object> Parameters { get; }
        IStatementBuilder AddLine(string keyword, params string[] columnNames);
        string RegisterParameter(object value);
    }
}