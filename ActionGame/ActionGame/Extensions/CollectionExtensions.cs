using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionGame.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this LinkedList<T> linkedList, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                linkedList.AddLast(item);
            }
        }

        public static T Second<T>(this IEnumerable<T> ien)
        {
            int i = 0;
            foreach (T item in ien)
            {
                if (i == 1)
                    return item;
                i++;
            }

            throw new InvalidOperationException("There's no second item.");
        }
    }
}
