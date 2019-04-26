using System.Collections.Generic;

namespace Toadstool
{
    public interface IBuildableQuery
    {
        string Build();
        IDictionary<string, object> Parameters { get; }
    }
}