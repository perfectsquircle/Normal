using System;
using System.Data;

namespace Toadstool
{
    internal interface IDataRecordMapperFactory
    {
        IDataRecordMapper CreateMapper(Type type);
    }
}