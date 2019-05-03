using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Toadstool
{
    internal static class ReflectionHelper
    {
        public static IDictionary<string, object> ToDictionary(object target)
        {
            return target
                .GetType()
                .GetRuntimeProperties()
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(target));
        }

        public static IDictionary<string, PropertyInfo> ToDictionaryOfProperties(object target)
        {
            return target
                .GetType()
                .GetRuntimeProperties()
                .ToDictionary(p => p.Name);
        }
    }
}