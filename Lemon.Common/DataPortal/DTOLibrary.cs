using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Winterspring.DataPortal
{
    public static class DTOLibrary
    {
        public static bool IsPrimitive(Type t)
        {
            if (t == typeof(Decimal))
                return true;
            if (t == typeof(String))
                return true;
            if (t == typeof(DateTime))
                return true;
            if (t == typeof(TimeSpan))
                return true;
            if (t == typeof(DateTimeOffset))
                return true;

            return t.IsPrimitive;
        }

        public static Type GetDTOType(Type t)
        {
            return IsPrimitive(t) ? t
                : IsDictionary(t) ? MapDictionary(t)
                : (DataMapper.Instance.MappedType(t) ?? t);
        }

        private static Type MapDictionary(Type dictionaryType)
        {
            var types = dictionaryType.GetGenericArguments();
            return typeof(Dictionary<,>).MakeGenericType(GetDTOType(types[0]), GetDTOType(types[1]));
        }

        public static string GetDTOTypeName(Type t)
        {
            return GetCompilableTypeName(GetDTOType(t));
        }

        //e.g. if t = Nullable<int>, t.Name will output Nullable`1.  This method converts it to something that makes sense to a compiler.
        public static string GetCompilableTypeName(Type t)
        {
            return t.IsGenericType
                ? String.Format("{0}<{1}>", t.FullName.Split('`')[0], String.Join(",", t.GetGenericArguments().Select(x => GetCompilableTypeName(x))))
                : t.FullName.Replace("+", ".");
        }

        public static string CreateEmptyCollection(Type t)
        {
            if (t.IsArray)
            {
                return String.Format("(new {0} {{ }})", GetCompilableTypeName(t));
            }
            else
            {
                return string.Format("(new {0}())", GetCompilableTypeName(t));
            }
        }


        //When creating an array, create an array with one element of that type
        public static object CreateTestInstance(Type t)
        {
            if (t == typeof(string))
            {
                return "abc";
            }
            else if (t.IsArray)
            {
                var array = Array.CreateInstance(t.GetElementType(), 1);
                ((IList)array)[0] = CreateTestInstance(t.GetElementType());
                return array;
            }
            else
            {
                return Activator.CreateInstance(t);
            }
        }

        public static bool IsCollectionType(Type t)
        {
            //Return true if array (except byte[]), list, or dictionary
            return ((t.IsArray && t.GetElementType() != typeof(byte))
                || (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>) || IsDictionary(t))));
        }

        public static bool IsDictionary(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static Type GetMappedDictionaryType(Type t)
        {
            if (!IsDictionary(t)) return t;
            var types = t.GetGenericArguments();
            return typeof(Dictionary<,>).MakeGenericType(GetDTOType(types[0]), GetDTOType(types[1]));
        }        
    }
}
