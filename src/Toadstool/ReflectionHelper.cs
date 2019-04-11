using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Toadstool
{
    internal static class ReflectionHelper
    {
        public static IDictionary<string, object> ToDictionary(object target)
        {
            var type = target.GetType();
            var properties = type.GetRuntimeProperties();
            return properties
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(target));
        }
    }
}