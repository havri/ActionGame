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
    }
}
