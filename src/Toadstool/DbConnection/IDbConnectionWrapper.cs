using System;
using System.Data;
using System.Threading.Tasks;

namespace Toadstool
{
    internal interface IDbConnectionWrapper : IDbTransactionWrapper, IDisposable
    {
        IDbCommand CreateCommand();
        CommandBehavior CommandBehavior { get; }
    }
}