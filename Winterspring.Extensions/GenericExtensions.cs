using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterspring.Extensions
{
    public static class GenericExtensions
    {
        public static bool IsIn<T>(this T t, params T[] items)
        {
            return items.Contains(t);
        }

        public static bool IsIn<T>(this T t, IEnumerable<T> items)
        {
            return items.Contains(t);
        }

        public static TReturn CastTo<TReturn>(this object obj)
        {
            return (TReturn)obj;
        }

        public static int ToInt(this object obj)
        {
            return (int)obj;
        }

        public static IEnumerable<TResult> Repeat<TResult>(Func<TResult> f, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return f();
            }
        }
    }
}
