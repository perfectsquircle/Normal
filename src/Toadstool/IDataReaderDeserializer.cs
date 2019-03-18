using System.Data;

namespace Toadstool
{
    public interface IDataReaderDeserializer
    {
        T Deserialize<T>(IDataReader dataReader);
    }
}