using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Toadstool
{
    public class DbResultBuilder
    {
        private readonly IDataReader _dataReader;
        private readonly IDataReaderDeserializer _dataReaderDeserializer;

        public DbResultBuilder(IDataReader dataReader, IDataReaderDeserializer dataReaderDeserializer)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException(nameof(dataReader));
            }
            _dataReader = dataReader;
            _dataReaderDeserializer = dataReaderDeserializer;
        }

        public IList<T> AsListOf<T>()
        {
            return AsEnumerableOf<T>().ToList();
        }

        public IEnumerable<T> AsEnumerableOf<T>()
        {
            using (_dataReader)
            {
                if (_dataReader.FieldCount == 0)
                {
                    yield break;
                }

                while (_dataReader.Read())
                {
                    yield return _dataReaderDeserializer.Deserialize<T>(_dataReader);
                }
                yield break;
            }
        }
    }
}