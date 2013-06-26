using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Winterspring.LanguageExtensions;
using Csla.Core;

namespace Winterspring.Lemon.Base
{
    public static class ChildChangedEventArgsExtensions
    {
        public static bool IsCollectionChange(this ChildChangedEventArgs e)
        {
            return e.ListChangedArgs != null || e.CollectionChangedArgs != null;
        }

        public static bool IsPropertyChange(this ChildChangedEventArgs e, string propertyName)
        {
            return e.PropertyChangedArgs != null && e.PropertyChangedArgs.PropertyName == propertyName;
        }

        public static bool IsPropertyChange(this ChildChangedEventArgs e, IPropertyInfo property)
        {
            return IsPropertyChange(e, property.Name);
        }
    }

    public static class ListChangedEventArgsExtensions
    {
        public static bool IsCollectionChange(this ListChangedEventArgs e)
        {
            return new[] { ListChangedType.Reset, ListChangedType.ItemAdded, ListChangedType.ItemDeleted }.Contains(e.ListChangedType);
        }

        public static bool IsPropertyChange(this ListChangedEventArgs e, string propertyName)
        {
            return e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor.Try(p => p.Name) == propertyName;
        }

        public static bool IsPropertyChange(this ListChangedEventArgs e, IPropertyInfo property)
        {
            return IsPropertyChange(e, property.Name);
        }

        public static T ChangedItem<T>(this ListChangedEventArgs e, IList<T> list) where T : class
        {
            if (e.NewIndex >= 0 && e.NewIndex < list.Count)
                return list[e.NewIndex];
            return null;
        }

        public static object ChangedObject(this ListChangedEventArgs e, IList list)
        {
            if (e.NewIndex >= 0 && e.NewIndex < list.Count)
                return list[e.NewIndex];
            return null;
        }

        public static string ChangedProperty(this ListChangedEventArgs e)
        {
            return e.PropertyDescriptor.Try(p => p.Name);
        }

    }
}
