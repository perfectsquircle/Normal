using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Normal
{
    internal class Member
    {
        private Delegate _getValue;

        protected Member(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;

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
        public virtual string Name { get; }
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
            _getValue ??= CreateGetValue(MemberInfo);
            return _getValue.DynamicInvoke(target);
        }

        public override bool Equals(object obj)
        {
            return obj is Member member &&
                   Name == member.Name &&
                   EqualityComparer<Type>.Default.Equals(Type, member.Type);
        }

        public override int GetHashCode()
        {
            int hashCode = 79417382;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Type);
            return hashCode;
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