using System;

namespace Normal
{
    internal interface IDataRecordMapperFactory
    {
        IDataRecordMapper CreateMapper(Type type);
    }
}