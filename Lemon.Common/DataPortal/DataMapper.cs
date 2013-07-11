using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Lemon.Common;

namespace Winterspring.DataPortal
{
    public class DataMapper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DataMapper()
        {
        }

        private static readonly DataMapper _Instance = new DataMapper();
        public static DataMapper Instance { get { return _Instance; } }

        public void Initialize()
        {
            //MEF catalog:  http://msdn.microsoft.com/en-us/library/dd460648.aspx#imports_and_exports_with_attributes
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load(new AssemblyName("Lemon.Library"))));
            _container = new CompositionContainer(catalog);

            //Fill the imports
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                log.Error(ex);
            }

            foreach (var i in initializers) { i.Value.InitializeDataMappings(); }
        }

        [ImportMany(typeof(IDataMapping))]
        private IEnumerable<Lazy<IDataMapping>> initializers;

        private CompositionContainer _container;

        private bool CanMap(Type t)
        {
            return t != null && MappedType(t) != null;
        }

        public bool CanMap(object obj)
        {
            return obj != null && CanMap(obj.GetType());
        }

        //Map either from DTO to BO or vice versa
        public object Map(object obj)
        {
            if (obj == null) return null;
            var type = obj.GetType();
            bool canMap = CanMap(type);
            if (!canMap)
                throw new InvalidOperationException("DataMapper not set up for type " + type.Name);

            try
            {
                if (DataTransferObjectLibrary.IsDictionary(type))
                {
                    return CreateMappedDictionary(obj, type);
                }
                else
                {
                    return _translateMethods[type](obj);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        private object CreateMappedDictionary(object oldDictionary, Type type)
        {
            var newDictionary = (IDictionary)Activator.CreateInstance(MappedType(type));
            var enumerator = ((IDictionary)oldDictionary).GetEnumerator();
            while (enumerator.MoveNext())
            {
                newDictionary.Add(CanMap(enumerator.Key) ? Map(enumerator.Key) : enumerator.Key,
                                  CanMap(enumerator.Value) ? Map(enumerator.Value) : enumerator.Value);
            }
            return newDictionary;
        }

        public Type MappedType(Type t)
        {
            return t == null ? null
                : DataTransferObjectLibrary.IsDictionary(t) ? GetMappedDictionaryType(t)
                : _translateType.ContainsKey(t) ? _translateType[t]
                : null;
        }

        private Type GetMappedDictionaryType(Type t)
        {
            if (!DataTransferObjectLibrary.IsDictionary(t)) return t;
            var types = t.GetGenericArguments();
            var t1 = MappedType(types[0]);
            var t2 = MappedType(types[1]);
            if (t1 == null && t2 == null) return null;  //No change
            return typeof(Dictionary<,>).MakeGenericType(t1 ?? types[0], t2 ?? types[1]);
        }

        public void RegisterMapping<T1, T2>(Func<T1, T2> f)
        {
            _translateMethods.Add(typeof(T1), o => f((T1)o));
            _translateType.Add(typeof(T1), typeof(T2));
        }

        private Dictionary<Type, Func<object, object>> _translateMethods = new Dictionary<Type, Func<object, object>>();
        private Dictionary<Type, Type> _translateType = new Dictionary<Type, Type>();
    }
}