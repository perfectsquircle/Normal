using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Normal
{
    internal class MemberMatch
    {
        private static readonly MethodInfo _dataRecordIsDbNull = typeof(IDataRecord).GetMethod("IsDBNull", new[] { typeof(int) });
        private static readonly MethodInfo _dataRecordGetValue = typeof(IDataRecord).GetMethod("GetValue", new[] { typeof(int) });
        private static readonly MethodInfo _dataRecordGetString = typeof(IDataRecord).GetMethod("GetString", new[] { typeof(int) });
        private static readonly MethodInfo _enumParse = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) });
        private static readonly MethodInfo _covertChangeType = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });

        public int ColumnIndex { get; set; }
        public Type ColumnType { get; set; }
        public Member Member { get; set; }

        public Expression GetColumnReader(ParameterExpression dataRecord)
        {
            return GetColumnReader(dataRecord, ColumnType, ColumnIndex, Member.Type);
        }

        public static Func<IDataRecord, TMember> GetColumnReader<TMember>(Type columnType, int columnIndex)
        {
            var dataRecordParameter = Parameter(typeof(IDataRecord), "dataRecord");
            var columnReaderExpression = GetColumnReader(dataRecordParameter, columnType, columnIndex, typeof(TMember));
            var lambda = Lambda<Func<IDataRecord, TMember>>(columnReaderExpression, false, dataRecordParameter);
            return lambda.Compile();
        }

        private static Expression GetColumnReader(ParameterExpression dataRecord, Type columnType, int columnIndex, Type memberType)
        {
            var columnIndexConstant = Constant(columnIndex);
            var getValueCall = Call(dataRecord, _dataRecordGetValue, columnIndexConstant);

            Expression getColumnValue;
            if (columnType == memberType || memberType.IsAssignableFrom(columnType))
            {
                // Convert(dataRecord.getValue(i), propertyType)
                getColumnValue = Convert(getValueCall, memberType);
            }
            else if (memberType.IsEnum)
            {
                if (columnType == typeof(string))
                {
                    // Enum.Parse(propertyType, dataRecord.GetString(columnIndex), true);
                    var enumParseCall = Call(_enumParse, Constant(memberType), Call(dataRecord, _dataRecordGetString, columnIndexConstant), Constant(true));
                    getColumnValue = Convert(enumParseCall, memberType);
                }
                else
                {
                    // Convert(dataRecord.getValue(i), propertyType)
                    getColumnValue = Convert(getValueCall, memberType);
                }
            }
            else
            {
                // Convert.ChangeType(dataRecord[columnIndex], propertyType);
                var changeTypeCall = Call(_covertChangeType, getValueCall, Constant(memberType));
                getColumnValue = Convert(changeTypeCall, memberType);
            }

            var isDbNullCall = Call(dataRecord, _dataRecordIsDbNull, columnIndexConstant);
            var condition = Condition(isDbNullCall, Default(memberType), getColumnValue);
            return condition;
        }
    }
}