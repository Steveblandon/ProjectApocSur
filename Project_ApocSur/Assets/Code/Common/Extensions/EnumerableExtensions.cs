namespace Projapocsur
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Adds items to a collection.
        /// </summary>
        /// <typeparam name="T"> The generic type.</typeparam>
        /// <param name="collection"> The collection to add items to.</param>
        /// <param name="items"> The items to add to the collection.</param>
        /// <param name="predicate"> Optional predicate to determine if an item should be added to the collection.</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items, Predicate<T> predicate = null)
        {
            foreach (var item in items)
            {
                if (predicate == null || (predicate != null && predicate(item)))
                {
                    collection.Add(item);
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> process)
        {
            foreach (var item in source)
            {
                process(item);
            }
        }

        public static T Find<T>(this IEnumerable<T> source, Predicate<T> match)
        {
            foreach (var item in source)
            {
                if (match(item))
                {
                    return item;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Checks if a collection is null or empty.
        /// </summary>
        /// <returns> True, if null or empty, false otherwise.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || (collection != null && collection.Count() == 0);

        /// <summary>
        /// Converts a collection to a set.
        /// </summary>
        /// <typeparam name="T"> The generic type.</typeparam>
        /// <param name="items"> The items to add to the set.</param>
        /// <returns> The set.</returns>
        public static ISet<T> ToSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }
    }
}