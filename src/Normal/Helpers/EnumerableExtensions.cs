using System;
using System.Collections.Generic;

namespace Normal
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Buffered<T>(this IEnumerable<T> enumerable)
        {
            return BufferImpl(enumerable.GetEnumerator(), new List<T>());
        }

        private static IEnumerable<T> BufferImpl<T>(IEnumerator<T> source, List<T> buffer)
        {
            int pos = 0;
            while (true)
            {
                if (pos == buffer.Count)
                    if (source.MoveNext())
                        buffer.Add(source.Current);
                    else
                        yield break;
                yield return buffer[pos++];
            }
        }
    }
}