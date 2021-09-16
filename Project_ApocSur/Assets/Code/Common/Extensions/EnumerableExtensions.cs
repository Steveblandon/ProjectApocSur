namespace Projapocsur.Common.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source != null)
            {
                foreach (object obj in source)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                foreach (T obj in source)
                {
                    return false;
                }
            }
            return true;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> process)
        {
            foreach (var item in source)
            {
                process(item);
            }
        }
    }
}