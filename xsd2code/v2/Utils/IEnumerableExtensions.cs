using System;
using System.Collections.Generic;

namespace xsd2code.v2.Utils
{
    public static class IEnumerableExtensions
    {
        public static T Reduce<T, U>(this IEnumerable<U> source, Func<U, T, T> func, T acc)
        {
            source.ForEach(i => acc = func(i, acc));
            return acc;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
