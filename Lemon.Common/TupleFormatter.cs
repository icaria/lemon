using System;

namespace Lemon.Common
{
    public static class TupleFormatter
    {
        public static string Format(params object[] values)
        {
            return String.Format("<{0}>", String.Join(", ", values)); 
        }
    }
}