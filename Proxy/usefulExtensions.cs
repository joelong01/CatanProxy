using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Catan.Proxy
{
    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> that)
        {
            if (that == null) return;
            foreach (T t in that)
            {
                collection.Add(t);
            }
        }
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            Contract.Requires(list != null);
            Contract.Requires(firstIndex >= 0 && firstIndex < list.Count);
            Contract.Requires(secondIndex >= 0 && secondIndex < list.Count);
            if (firstIndex == secondIndex)
            {
                return;
            }
            T temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}
