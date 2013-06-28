using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Winterspring.LanguageExtensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Like ToDictionary, but if there are two elements with the same key, it only uses the first one
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionarySafe<TEnumerable, TKey, TValue>(this IEnumerable<TEnumerable> e, Func<TEnumerable, TKey> fk, Func<TEnumerable, TValue> fv)
        {
            var d = new Dictionary<TKey, TValue>();
            foreach (var i in e)
            {
                var key = fk(i);
                if(!d.ContainsKey(key))
                    d.Add(key, fv(i));
            }
            return d;
        }

        public static HashSet<TResult> ToHashSet<TEnumerable, TResult>(this IEnumerable<TEnumerable> e, Func<TEnumerable, TResult> mapping)
        {
            return e.Select(mapping).ToHashSet();
        }
        
        public static HashSet<TEnumerable> ToHashSet<TEnumerable>(this IEnumerable<TEnumerable> e)
        {
            return new HashSet<TEnumerable>(e);                        
        }

        public static T2? MaxOrNullStruct<T, T2>(this IEnumerable<T> ie, Func<T, T2> selector) where T2 : struct
        {
            Debug.Assert(selector != null, "selector != null");

            if (ie != null && ie.Any())
            {
                return ie.Max(selector);
            }
            else
                return null;
        }

        public static T2 MaxOrNullClass<T, T2>(this IEnumerable<T> ie, Func<T, T2> selector) where T2 : class
        {
            Debug.Assert(selector != null, "selector != null");

            if (ie != null && ie.Any())
            {
                return ie.Max(selector);
            }
            else
                return null;
        }

        public static TSrc ArgMax<TSrc, TArg>(this IEnumerable<TSrc> ie, Converter<TSrc, TArg> fn)
            where TArg : IComparable<TArg>
        {
            IEnumerator<TSrc> e = ie.GetEnumerator();
            if (!e.MoveNext())
                throw new InvalidOperationException("Sequence has no elements.");

            TSrc t_try, t = e.Current;
            if (!e.MoveNext())
                return t;

            TArg v, max_val = fn(t);
            do
            {
                if ((v = fn(t_try = e.Current)).CompareTo(max_val) > 0)
                {
                    t = t_try;
                    max_val = v;
                }
            }
            while (e.MoveNext());
            return t;
        }

        public static TSrc ArgMaxOrNull<TSrc, TArg>(this IEnumerable<TSrc> ie, Converter<TSrc, TArg> fn)
            where TArg : IComparable<TArg>
            where TSrc : class
        {
            IEnumerator<TSrc> e = ie.GetEnumerator();
            if (!e.MoveNext())
                return null;

            TSrc t_try, t = e.Current;
            if (!e.MoveNext())
                return t;

            TArg v, max_val = fn(t);
            do
            {
                if ((v = fn(t_try = e.Current)).CompareTo(max_val) > 0)
                {
                    t = t_try;
                    max_val = v;
                }
            }
            while (e.MoveNext());
            return t;
        }

        public static TSrc ArgMin<TSrc, TArg>(this IEnumerable<TSrc> ie, Converter<TSrc, TArg> fn)
            where TArg : IComparable<TArg>
        {
            IEnumerator<TSrc> e = ie.GetEnumerator();
            if (!e.MoveNext())
                throw new InvalidOperationException("Sequence has no elements.");

            TSrc t_try, t = e.Current;
            if (!e.MoveNext())
                return t;

            TArg v, min_val = fn(t);
            do
            {
                if ((v = fn(t_try = e.Current)).CompareTo(min_val) < 0)
                {
                    t = t_try;
                    min_val = v;
                }
            }
            while (e.MoveNext());
            return t;
        }

        public static TSrc ArgMinOrNull<TSrc, TArg>(this IEnumerable<TSrc> ie, Converter<TSrc, TArg> fn)
            where TArg : IComparable<TArg>
            where TSrc : class
        {
            IEnumerator<TSrc> e = ie.GetEnumerator();
            if (!e.MoveNext())
                return null;

            TSrc t_try, t = e.Current;
            if (!e.MoveNext())
                return t;

            TArg v, min_val = fn(t);
            do
            {
                if ((v = fn(t_try = e.Current)).CompareTo(min_val) < 0)
                {
                    t = t_try;
                    min_val = v;
                }
            }
            while (e.MoveNext());
            return t;
        }

        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            foreach (T el in list)
            {
                if (predicate(el))
                    yield break;
                yield return el;
            }
        }

        public static IEnumerable<T> TakeUntilIncluding<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            foreach (T el in list)
            {
                yield return el;
                if (predicate(el))
                    yield break;
            }
        }

        /// <summary>
        /// Like foreach, but also gives you a zero-based counter of the elements of this enumerable.
        /// </summary>
        public static void ForEachWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> handler)
        {
            int idx = 0;
            foreach (T item in enumerable)
                handler(item, idx++);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> handler)
        {
            if (enumerable == null) return;
            foreach (T item in enumerable)
            {
                handler(item);
            }
        }
    }
}