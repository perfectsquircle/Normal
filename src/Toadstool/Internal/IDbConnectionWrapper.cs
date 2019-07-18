using System;
using System.Data;
using System.Threading.Tasks;

namespace Toadstool
{
    internal interface IDbConnectionWrapper : IDisposable
    {
        IDbCommand CreateCommand();
        CommandBehavior CommandBehavior { get; }
    }
}