using System;
using System.Collections;
using System.Data;

// STOLEN FROM https://gist.github.com/jwatney/2335164

namespace Moq.DataExtensions
{
    public static class MockRepositoryDataExtensions
    {
        public static Mock<IDbCommand> CreateIDbCommand(this MockRepository factory)
        {
            var command = factory.Create<IDbCommand>();

            command.SetupAllProperties();
            command.Setup(c => c.CreateParameter()).Returns(() => factory.CreateIDbDataParameter().Object);
            command.Setup(c => c.Parameters).Returns(factory.CreateIDataParameterCollection().Object);

            return command;
        }

        public static Mock<IDataParameterCollection> CreateIDataParameterCollection(this MockRepository factory)
        {
            var list = new ArrayList(); // ArrayList more closely matches IDataParameterCollection.
            var parameters = factory.Create<IDataParameterCollection>();

            parameters.Setup(p => p.Add(It.IsAny<IDataParameter>())).Returns((IDataParameter p) => list.Add(p));
            parameters.Setup(p => p[It.IsAny<int>()]).Returns((int i) => list[i]);
            parameters.Setup(p => p.Count).Returns(() => list.Count);

            return parameters;
        }

        public static Mock<IDbDataParameter> CreateIDbDataParameter(this MockRepository factory)
        {
            var parameter = factory.Create<IDbDataParameter>();

            parameter.SetupAllProperties();

            return parameter;
        }

        public static Mock<IDataRecord> CreateIDataRecord(this MockRepository factory, params object[] fields)
        {
            var record = factory.Create<IDataRecord>();

            for (var index = 0; index < fields.Length; index++)
            {
                var column = fields[index];
                var type = column.GetType();
                var name = (string)type.GetProperty("Name").GetValue(column, null);
                var value = type.GetProperty("Value").GetValue(column, null);

                record.Setup(r => r.IsDBNull(index)).Returns(value == DBNull.Value);
                record.Setup(r => r.GetOrdinal(name)).Returns(index);
                record.Setup(r => r[index]).Returns(value);
                record.Setup(r => r[name]).Returns(value);
                record.Setup(r => r.GetName(index)).Returns(name);
                record.Setup(r => r.GetString(index)).Returns(() => (string)value);
                record.Setup(r => r.GetInt32(index)).Returns(() => (int)value);
            }

            record.SetupGet(r => r.FieldCount).Returns(fields.Length);

            return record;
        }
    }
}