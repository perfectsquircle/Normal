using System.Data;

namespace Normal
{
    public interface IDataRecordMapper<T>
    {
        T MapDataRecord(IDataRecord dataRecord);
    }
}