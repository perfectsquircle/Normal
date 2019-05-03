using System.Collections.Generic;

namespace Toadstool
{
    public interface IQueryBuilder
    {
        string Build();
        IDictionary<string, object> Parameters { get; }
        IQueryBuilder AddLine(string keyword, params string[] columnNames);
        string RegisterParameter(object value);
    }
}