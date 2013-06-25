using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon.LanguageExtensions
{
    public class ArrayUtil
    {
        public static bool SetEquals<T>(IEnumerable<T> a1, IEnumerable<T> a2)
        {
            if (ReferenceEquals(a1, a2)) return true;
            if (a1 == null || a2 == null) return false;
            return a1.ToHashSet().SetEquals(a2);
        }

        public static bool ArrayEquals<T>(T[] a1, T[] a2)
        {
            if (a1 == null && a2 == null) return true;
            if (a1 == null || a2 == null) return false;
            if (a1.Length != a2.Length) return false;
            for (int i = 0; i < a1.Length; ++i)
            {
                if (!Equals(a1[i], a2[i]))
                    return false;
            }
            return true;
        }

        //Returns:
        // <0 if a1 < a2
        // =0 if a1 == a2
        // >0 if a1 > a2
        //Treat null as the minimum possible value
        //Treat a shorter array as smaller
        public static int CompareArray<T>(T[] a1, T[] a2) where T : IComparable
        {

            if (a1 == null && a2 == null) return 0;
            if (a1 == null) return -1;
            if (a2 == null) return 1;
            if (a1.Length < a2.Length) return -1;
            if (a1.Length > a2.Length) return 1;
            //Check from the start (big-endian)
            for (int i = 0; i < a1.Length; ++i)
            {
                int c = a1[i].CompareTo(a2[i]);
                if (c != 0) return c;
            }
            //All entries are the same
            return 0;
        }

        public static bool ArrayEquals<T>(IEnumerable<T> a1, IEnumerable<T> a2)
        {
            IEnumerator<T> e1 = a1.GetEnumerator();
            IEnumerator<T> e2 = a2.GetEnumerator();

            while (true)
            {
                bool more1 = e1.MoveNext();
                bool more2 = e2.MoveNext();

                //There's a mismatch in the collection size
                if (more1 != more2) return false;
                //We've run through all the elements and they match
                if (more1 == false) return true;
                //If the elements here don't match up
                if (!Equals(e1.Current, e2.Current))
                    return false;
            }
        }

        public static T[] ArrayDeepCopy<T>(T[] array1) where T : ICloneable
        {
            if (array1 == null) return null;
            T[] array2 = new T[array1.Length];
            for (int i = 0; i < array1.Length; ++i)
            {
                array2[i] = (T)array1[i].Clone();
            }
            return array2;
        }

    }
}
