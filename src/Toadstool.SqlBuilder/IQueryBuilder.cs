using System.Collections.Generic;

namespace Toadstool
{
    public interface IQueryBuilder
    {
        string Build();
        IDictionary<string, object> Parameters { get; }
    }
}