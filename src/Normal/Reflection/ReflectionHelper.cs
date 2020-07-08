using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Normal
{
    internal static class ReflectionHelper
    {
        public static IDictionary<string, object> ToDictionary(object target)
        {
            return Member
                .GetMembers(target.GetType())
                .Where(m => m.CanRead)
                .ToDictionary(m => m.Name, m => m.GetValue(target));
        }

        public static ConstructorInfo GetConstructor(Type targetType, object[] arguments)
        {
            var argumentTypes = arguments.Select(a => a.GetType()).ToArray();
            var constructor = targetType.GetConstructor(argumentTypes); ;
            if (constructor == null)
            {
                var argumentTypeStrings = string.Join(",", argumentTypes.Select(t => t.ToString()));
                throw new NotSupportedException($"No constructor found: {targetType}({argumentTypeStrings})");
            }

            return constructor;
        }

    }
}