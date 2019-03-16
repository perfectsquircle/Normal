using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Toadstool
{
    public class DbResultBuilder
    {
        private readonly IDataReader _dataReader;

        public DbResultBuilder(IDataReader dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException(nameof(dataReader));
            }
            _dataReader = dataReader;
        }

        public IList<T> AsList<T>()
        {
            return As<T>().ToList();
        }

        public IEnumerable<T> As<T>()
        {
            using (_dataReader)
            {
                if (_dataReader.FieldCount == 0)
                {
                    yield break;
                }

                while (_dataReader.Read())
                {
                    yield return Deserialize<T>(_dataReader);
                }
            }
        }

        public static T Deserialize<T>(IDataReader dataReader)
        {
            T obj = default(T);
            obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo prop in obj.GetType().GetRuntimeProperties()) // TODO: cache this
            {
                if (!object.Equals(dataReader[prop.Name], DBNull.Value))
                {
                    prop.SetValue(obj, dataReader[prop.Name], null);
                }
            }
            return obj;
        }
    }
}