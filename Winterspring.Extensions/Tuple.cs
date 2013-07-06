using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterspring.Extensions
{
    //This is a poor man's Tuple for when we're using old versions of .NET
    public class CustomTuple
    {
        public static CustomTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { return new CustomTuple<T1, T2>(item1, item2); }
        public static CustomTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) { return new CustomTuple<T1, T2, T3>(item1, item2, item3); }
    }

    public class CustomTuple<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public CustomTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        protected bool Equals(CustomTuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tuple<T1, T2>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T1>.Default.GetHashCode(Item1) * 397) ^ EqualityComparer<T2>.Default.GetHashCode(Item2);
            }
        }
    }

    public class CustomTuple<T1, T2, T3>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }

        public CustomTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        protected bool Equals(CustomTuple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(Item2, other.Item2) && EqualityComparer<T3>.Default.Equals(Item3, other.Item3);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tuple<T1, T2, T3>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EqualityComparer<T1>.Default.GetHashCode(Item1);
                hashCode = (hashCode*397) ^ EqualityComparer<T2>.Default.GetHashCode(Item2);
                hashCode = (hashCode*397) ^ EqualityComparer<T3>.Default.GetHashCode(Item3);
                return hashCode;
            }
        }
    }
}
