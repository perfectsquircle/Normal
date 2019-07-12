using System.Collections.Generic;
using System.Linq;
using FastMember;

namespace Toadstool
{
    internal static class ReflectionHelper
    {
        public static IDictionary<string, object> ToDictionary(object target)
        {
            var typeAccessor = TypeAccessor.Create(target.GetType());
            return typeAccessor
                .GetMembers()
                .Where(m => m.CanRead)
                .ToDictionary(m => m.Name, m => typeAccessor[target, m.Name]);
        }
    }
}