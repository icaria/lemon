using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon.LanguageExtensions
{
    public static class TryExtensions
    {
        //If the source is null, this returns null.  Otherwise, it returns the result of the function in the second parameter.
        //Inspired by http://api.rubyonrails.org/classes/Object.html#method-i-try
        //This allows you to use shortcuts to condense your code, e.g. instead of:
        //
        //ProductCacheObject o = GetObject(prodId);
        //return o == null ? string.Empty : o.DefaultSublocation;
        //
        //You can use:  
        //return GetObject(prodId).Try(x => x.DefaultSublocation) ?? string.Empty;
        //
        //This lets you put this idiom of code on one line, without doing the work to fetch the source twice,
        //and without cluttering up your namespace with a meaningless temporary variable name.

        public static TReturn Try<TSource, TReturn>(this TSource? t, Func<TSource, TReturn> f) where TReturn : class
            where TSource: struct
        {
            return t == null ? null : f(t.Value);
        }

        public static TReturn? Try<TSource, TReturn>(this TSource? t, Func<TSource, TReturn?> f) 
            where TReturn: struct
            where TSource: struct
        {
            return t == null ? null : f(t.Value);
        }

        public static TReturn Try<TSource, TReturn>(this TSource t, Func<TSource, TReturn> f) where TReturn : class
        {
            return t == null ? null : f(t);
        }

        public static TReturn TryOr<TSource, TReturn>(this TSource t, Func<TSource, TReturn> f, Func<TReturn> g)
        {
            return t == null ? g() : f(t);
        }

        public static bool? Try<TSource>(this TSource t, Func<TSource, bool> f) where TSource : class
        {
            return t == null ? null : (bool?)f(t);
        }

        public static bool? Try<TSource>(this TSource t, Func<TSource, bool?> f) where TSource : class
        {
            return t == null ? null : f(t);
        }

        public static int? Try<TSource>(this TSource t, Func<TSource, int> f) where TSource : class
        {
            return t == null ? null : (int?)f(t);
        }

        public static int? Try<TSource>(this TSource t, Func<TSource, int?> f) where TSource : class
        {
            return t == null ? null : f(t);
        }

        public static decimal? Try<TSource>(this TSource t, Func<TSource, decimal> f) where TSource : class
        {
            return t == null ? null : (decimal?)f(t);
        }

        public static decimal? Try<TSource>(this TSource t, Func<TSource, decimal?> f) where TSource : class
        {
            return t == null ? null : f(t);
        }

        public static bool TryDo<T>(this T t, Action<T> f)
            where T : class
        {
            if (t != null)
            {
                f(t);
                return true;
            }
            return false;
        }

        public static void TryDo<T>(this T t, Action<T> present, Action notPresent)
            where T : class
        {
            if (t != null)
            {
                present(t);
            }
            else
            {
                notPresent();
            }
        }

        public static bool TryDo<T>(this T? t, Action<T?> f)
            where T : struct
        {
            if (t != null)
            {
                f(t);
                return true;
            }
            return false;
        }

    }
}
