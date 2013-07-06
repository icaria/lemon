using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Winterspring.Extensions
{
    public static class DictionaryExtensions
    {
        public static T2 GetWithDefault<T1, T2>(this IDictionary<T1, T2> x, T1 key, T2 defaultValue)
        {
            if (key != null && x.ContainsKey(key))
                return x[key];
            return defaultValue;
        }

        public static T2 GetOrNull<T1, T2>(this IDictionary<T1, T2> x, T1 key) where T2 : class
        {
            if (key != null && x.ContainsKey(key))
                return x[key];
            return null;
        }

        public static T2 GetCreateDefault<T1, T2>(this IDictionary<T1, T2> x, T1 key, Func<T2> del)
        {
            if (!x.ContainsKey(key))
                x[key] = del();
            return x[key];
        }

        //Update or insert
        public static void Upsert<T1, T2>(this IDictionary<T1, T2> x, T1 key, T2 value)
        {
            if (x.ContainsKey(key))
            {
                x[key] = value;
            }
            else
            {
                x.Add(key, value);
            }
        }

        public static void Upsert<T1, T2>(this IDictionary<T1, List<T2>> x, T1 key, T2 value)
        {
            if (x.ContainsKey(key))
            {
                x[key].Add(value);
            }
            else
            {
                x[key] = new List<T2>(new[] { value });
            }
        }

        public static Dictionary<T1, decimal> Sum<T1>(this Dictionary<T1, decimal> self, Dictionary<T1, decimal> other)
        {
            var d = self.Keys.Union(other.Keys).ToDictionary(x => x, x => 0M);
            foreach (var kv in self)
                d[kv.Key] += kv.Value;
            foreach (var kv in other)
                d[kv.Key] += kv.Value;
            return d;
        }

        public static Dictionary<T1, decimal> Difference<T1>(this Dictionary<T1, decimal> self, Dictionary<T1, decimal> other)
        {
            var d = self.Keys.Union(other.Keys).ToDictionary(x => x, x => 0M);
            foreach (var kv in self)
                d[kv.Key] += kv.Value;
            foreach (var kv in other)
                d[kv.Key] -= kv.Value;
            return d;
        }

        public static Dictionary<T1, decimal> Negative<T1>(this Dictionary<T1, decimal> self)
        {
            return self.ToDictionary(x => x.Key, x => -x.Value);
        }

        //Note that unlike most LINQ-like methods, this method is not pure, for efficiency.
        public static Dictionary<T1, T2> RemoveWhere<T1, T2>(this Dictionary<T1, T2> self, Predicate<KeyValuePair<T1, T2>> where)
        {
            foreach (var deleteKey in self.Where(kv => where(kv)).ToArray()) { self.Remove(deleteKey.Key); }
            return self;
        }

        //Note that unlike most LINQ-like methods, this method is not pure, for efficiency.
        public static IDictionary<T1, T2> RemoveWhere<T1, T2>(this IDictionary<T1, T2> self, Predicate<KeyValuePair<T1, T2>> where)
        {
            foreach (var deleteKey in self.Where(kv => where(kv)).ToArray()) { self.Remove(deleteKey); }
            return self;
        }

    }
}