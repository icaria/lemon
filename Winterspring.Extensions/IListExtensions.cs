using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterspring.Extensions
{
    public static class IListExtensions
    {
        public static IList<T> RemoveWhere<T>(this IList<T> list, Predicate<T> filter)
        {
            list.Where(x => filter(x)).ToArray().ForEach(x => list.Remove(x));
            return list;
        }

        public static IList<T> RemoveWhere<T>(this IList<T> list, IEnumerable<T> toRemove)
        {
            toRemove.Distinct().ToArray().ForEach(x => list.Remove(x));
            return list;
        }

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> toAdd)
        {
            foreach (var l in toAdd)
                list.Add(l);
            return list;
        }

    }
}
