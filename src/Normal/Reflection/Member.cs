using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Normal
{
    internal sealed class Member
    {
        private static readonly MethodInfo _dataRecordIsDbNull = typeof(IDataRecord).GetMethod("IsDBNull", new[] { typeof(int) });
        private static readonly MethodInfo _dataRecordGetValue = typeof(IDataRecord).GetMethod("GetValue", new[] { typeof(int) });
        private static readonly MethodInfo _dataRecordGetString = typeof(IDataRecord).GetMethod("GetString", new[] { typeof(int) });
        private static readonly MethodInfo _enumParse = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) });
        private static readonly MethodInfo _covertChangeType = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });
        private readonly Delegate _getValue;

        private Member(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            _getValue = CreateGetValue(memberInfo);

            if (memberInfo is FieldInfo)
            {
                var f = memberInfo as FieldInfo;
                Name = f.Name;
                Type = f.FieldType;
                CanWrite = f.IsPublic;
                CanRead = f.IsPublic;
            }
            else if (memberInfo is PropertyInfo)
            {
                var p = memberInfo as PropertyInfo;
                Name = p.Name;
                Type = p.PropertyType;
                CanWrite = p.CanWrite;
                CanRead = p.CanRead;
            }
        }

        public MemberInfo MemberInfo { get; }
        public string Name { get; }
        public Type Type { get; }
        public bool CanWrite { get; }
        public bool CanRead { get; }

        public static IEnumerable<Member> GetMembers(Type targetType)
        {
            return targetType
                .GetFields()
                .Select(f => new Member(f)).Concat(
                    targetType
                        .GetProperties()
                        .Select(p => new Member(p))
                );
        }

        public Attribute GetAttribute(Type attributeType, bool inherit)
        {
            return Attribute.GetCustomAttribute(MemberInfo, attributeType, inherit);
        }

        public T GetAttribute<T>(bool inherit = false) where T : Attribute
        {
            return MemberInfo.GetCustomAttribute<T>(inherit);
        }

        public object GetValue(object target)
        {
            return _getValue.DynamicInvoke(target);
        }

        public Expression GetColumnReader(ParameterExpression dataRecord, Type columnType, int columnIndex)
        {
            return GetColumnReader(dataRecord, columnType, columnIndex, Type);
        }

        public static Func<IDataRecord, TMember> GetColumnReader<TMember>(Type columnType, int columnIndex)
        {
            var dataRecordParameter = Parameter(typeof(IDataRecord), "dataRecord");
            var columnReaderExpression = GetColumnReader(dataRecordParameter, columnType, columnIndex, typeof(TMember));
            var lambda = Lambda<Func<IDataRecord, TMember>>(columnReaderExpression, false, dataRecordParameter);
            return lambda.Compile();
        }

        public static Expression GetColumnReader(ParameterExpression dataRecord, Type columnType, int columnIndex, Type memberType)
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

        private static Delegate CreateGetValue(MemberInfo memberInfo)
        {
            var target = Parameter(memberInfo.DeclaringType, "target");
            var memberAccess = MakeMemberAccess(target, memberInfo);
            var lambda = Lambda(memberAccess, false, target);
            return lambda.Compile();
        }
    }
}