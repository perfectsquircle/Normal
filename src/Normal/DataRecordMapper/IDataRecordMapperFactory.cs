using System;
using System.Data;

namespace Normal
{
    internal interface IDataRecordMapperFactory
    {
        IDataRecordMapper CreateMapper(Type type);
    }
}