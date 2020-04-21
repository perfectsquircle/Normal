using System;

namespace Normal
{
    internal interface IDataRecordMapperFactory
    {
        IDataRecordMapper<T> CreateMapper<T>();
        IDataRecordMapperFactory UseCustomMapper<T>(IDataRecordMapper<T> mapper);
    }
}