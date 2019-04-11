using System.Data;

namespace Toadstool
{
    public interface IDataRecordDeserializer
    {
        T Deserialize<T>(IDataRecord dataRecord);
    }
}