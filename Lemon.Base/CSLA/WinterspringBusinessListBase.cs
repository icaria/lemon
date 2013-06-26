using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Winterspring.LanguageExtensions;
using Csla;
using Csla.Core;
using Csla.Properties;

namespace Winterspring.Lemon.Base
{
    [Serializable()]
    public abstract class WinterspringBusinessListBase<T, C> : BusinessBindingListBase<T, C>, IWinterspringBusinessListBase
        where T : WinterspringBusinessListBase<T, C>
        where C : class, IEditableBusinessObject
    {
        public bool HasDataConflict { get; set; }

        public virtual string GetHumanReadableName()
        {
            return Util.CodeToHumanReadable(this.GetType().Name);
        }

        #region Sorting

        [NonSerialized]
        private PropertyDescriptor _sortProperty;
        private ListSortDirection _sortDirection;
        private bool _isSorted;

        // a cache of functions that perform the sorting
        // for a given type, property, and sort direction
        static readonly Dictionary<string, Func<IList<C>, IEnumerable<C>>> CachedOrderByExpressions = new Dictionary<string, Func<IList<C>, IEnumerable<C>>>();

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            /*
             Look for an appropriate sort method in the cache if not found .
             Call CreateOrderByMethod to create one. 
             Apply it to the original list.
             Notify any bound controls that the sort has been applied.
             */

            _sortProperty = prop;
            _sortDirection = direction;

            var orderByMethodName = _sortDirection == ListSortDirection.Ascending ? "OrderBy" : "OrderByDescending";
            var cacheKey = typeof(C).GUID + prop.Name + orderByMethodName;

            if (!CachedOrderByExpressions.ContainsKey(cacheKey))
            {
                CreateOrderByMethod(prop, orderByMethodName, cacheKey);
            }

            //Disable notifications, rebuild, and re-enable notifications
            var sortedList = CachedOrderByExpressions[cacheKey](this).ToArray();
            bool oldRaise = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            try
            {
                ClearItems();
                sortedList.ForEach(Add);
            }
            finally
            {
                RaiseListChangedEvents = oldRaise;
                ResetBindings();
            }

            _isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }


        private void CreateOrderByMethod(PropertyDescriptor prop, string orderByMethodName, string cacheKey)
        {
            //Create a generic method implementation for IEnumerable<T>. Cache it.

            var sourceParameter = Expression.Parameter(typeof(IList<C>), "source");
            var lambdaParameter = Expression.Parameter(typeof(C), "lambdaParameter");
            var accesedMember = typeof(C).GetProperty(prop.Name);
            var propertySelectorLambda = Expression.Lambda(Expression.MakeMemberAccess(lambdaParameter, accesedMember), lambdaParameter);
            var orderByMethod = typeof(Enumerable).GetMethods().Single(a => a.Name == orderByMethodName &&
                a.GetParameters().Length == 2).MakeGenericMethod(typeof(C), prop.PropertyType);

            var orderByExpression = Expression.Lambda<Func<IList<C>, IEnumerable<C>>>(Expression.Call(orderByMethod, new Expression[] { sourceParameter, 
                    propertySelectorLambda }), sourceParameter);

            CachedOrderByExpressions.Add(cacheKey, orderByExpression.Compile());
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _sortProperty = base.SortPropertyCore;
            _sortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }

        #endregion

#if DEBUG

        //This is a debugging tool that uses reflection to help find out what's bound to this list.
        private BindingSource GetBindingSource()
        {
            var eventField = typeof(BindingList<C>).GetField("onListChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            var eventHandler = eventField.GetValue(this) as ListChangedEventHandler;
            var delegates = eventHandler.GetInvocationList();
            return delegates.Select(d => d.Target).OfType<BindingSource>().FirstOrDefault();
        }

#endif

        #region OnListChanged

        protected virtual bool HasDoPropertyChanged { get { return false; } }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            //Run the basic one first so that it updates CurrencyManager
            //Otherwise the DataGridView may not be in synch when we're doing other things based on the ListChangedEvent
            base.OnListChanged(e);

            if (HasDoPropertyChanged)
            {
                //Now run our own PropertyChangedActionManager stuff
                //Run the property changed handler for the "List Listeners" (i.e SalesOrderPickLineList)
                DoPropertyChanged(this, e.IsCollectionChange() ? String.Empty : e.ChangedProperty());

                if (!e.IsCollectionChange())
                {
                    //If this is a child object change... run it for the "Child listeners as well (i.e SalesOrderPickLine)
                    DoPropertyChanged(e.ChangedObject(this), e.ChangedProperty());
                }
            }

            OnAfterListChanged(e);
        }

        public void InsertItem(int index)
        {
            RaiseListChangedEvents = false;

            var newItem = AddNew();
            Remove(newItem);

            RaiseListChangedEvents = true;

            Insert(index, newItem);
        }

        //public override System.Threading.Tasks.Task<T> SaveAsync()
        //{
        //    //This is a strange hack.  Even though this method is called SaveAsync, we're making it save synchronously,
        //    //since this is the only hook that we have since CSLA BusinessBindingListBase calls SaveAsync() and there's no isSync flag.
        //    return SaveAsync(true);
        //}
        
        public async System.Threading.Tasks.Task<T> SaveAsync(bool isSync)
        {
            T result;
            if (this.IsChild)
                throw new InvalidOperationException(Resources.NoSaveChildException);

            if (EditLevel > 0)
                throw new InvalidOperationException(Resources.NoSaveEditingException);

            if (!IsValid)
                throw new Csla.Rules.ValidationException(Resources.NoSaveInvalidException);

            if (IsBusy)
                // TODO: Review this resource text
                throw new InvalidOperationException(Resources.BusyObjectsMayNotBeSaved);

            if (IsDirty)
            {
                if (isSync)
                    result = DataPortal.Update<T>((T)this);
                else
                    result = await DataPortal.UpdateAsync<T>((T)this);
            }
            else
                result = (T)this;
            OnSaved(result, null, null);
            return result;
        }


        protected void RemoveWithoutDeletingItem(C child)
        {
            bool doSpecialHandling = true;

            if (doSpecialHandling)
            {
                //This is a hackish way to remove an item from the list without marking it to be deleted from the database, e.g. if transferring the child from one parent to another.
                //Looking at the CSLA code (as of version 4.3.13 and 4.5.10.0), there's a private boolean field _completelyRemoveChild in BusinessBindingListBase that controls when not to 
                //do this delete, so we can temporarily ensure that the flag is on while we delete it.
                var completelyRemoveChildField = typeof(BusinessBindingListBase<T, C>).GetField("_completelyRemoveChild", BindingFlags.NonPublic | BindingFlags.Instance);
                var oldValue = completelyRemoveChildField.GetValue(this);
                completelyRemoveChildField.SetValue(this, true);
                Remove(child);  //This should not change the value
                completelyRemoveChildField.SetValue(this, oldValue);
            }
            else
            {
                //Depending on changes of the implementation details of CSLA, this might not be necessary.  If so just delete it.
                //The unit test WorkOrderTest.TestMoveChildTo can help you determine this.
                //In the past the special handling was necessary, then wasn't necessary, then was necessary again.
                Remove(child);
            }

        }

        public void SwapItem(int index1, int index2)
        {
            var item1 = this[index1];
            var item2 = this[index2];

            var oldListChangedEvents = RaiseListChangedEvents;

            RaiseListChangedEvents = false;
            this[index2] = item1;
            this[index1] = item2;
            RaiseListChangedEvents = oldListChangedEvents;
            if (oldListChangedEvents)
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void DoPropertyChanged(object source, string propertyName)
        {
            DoPropertyChangedSyncActions(source, propertyName);
            DoPropertyChangedDoActions(source, propertyName);
        }

        private void DoPropertyChangedDoActions(object source, string propertyName)
        {
            if (DoMode == PropertyChangedActionMode.Batch)
            {
                if (!_batchedDoActions.Any(x => Tuple.Create(source, propertyName) == x))
                {
                    _batchedDoActions.Add(Tuple.Create(source, propertyName));
                }
            }
            else if (DoMode == PropertyChangedActionMode.Normal)
            {
                RunPropertyChangedDoActions(source, propertyName);
            }
        }

        private void DoPropertyChangedSyncActions(object source, string propertyName)
        {
            if (SyncMode == PropertyChangedActionMode.Batch)
            {
                if (!_batchedSyncActions.Any(x => Tuple.Create(source, propertyName) == x))
                {
                    _batchedSyncActions.Add(Tuple.Create(source, propertyName));
                }
            }
            else if (SyncMode == PropertyChangedActionMode.Normal)
            {
                RunPropertyChangedSyncActions(source, propertyName);
            }
        }

        private readonly HashSet<Tuple<object, string>> _batchedSyncActions = new HashSet<Tuple<object, string>>();
        private readonly HashSet<Tuple<object, string>> _batchedDoActions = new HashSet<Tuple<object, string>>();

        private PropertyChangedActionMode _syncMode;
        public PropertyChangedActionMode SyncMode
        {
            get { return _syncMode; }
            set
            {
                if (_syncMode != value)
                {
                    //If we're changing from batch to normal, then fire all the queued up changes
                    if (_syncMode == PropertyChangedActionMode.Batch && value == PropertyChangedActionMode.Normal)
                    {
                        //Run all the property changes.  More might get added in as we're doing this
                        while (_batchedSyncActions.Count > 0)
                        {
                            var action = _batchedSyncActions.First();
                            _batchedSyncActions.Remove(action);
                            RunPropertyChangedSyncActions(action.Item1, action.Item2);
                        }
                    }
                    _syncMode = value;
                }
            }
        }

        private PropertyChangedActionMode _doMode;
        public PropertyChangedActionMode DoMode
        {
            get { return _doMode; }
            set
            {
                if (_doMode != value)
                {
                    //If we're changing from batch to normal, then fire all the queued up changes
                    if (_doMode == PropertyChangedActionMode.Batch && value == PropertyChangedActionMode.Normal)
                    {
                        //Run all the property changes.  More might get added in as we're doing this
                        while (_batchedDoActions.Count > 0)
                        {
                            var action = _batchedDoActions.First();
                            _batchedDoActions.Remove(action);
                            RunPropertyChangedDoActions(action.Item1, action.Item2);
                        }
                    }
                    _doMode = value;
                }
            }
        }

        private void RunPropertyChangedSyncActions(object source, string propertyName)
        {
            RunPropertyChangedActions<T>(source, propertyName, true);
            RunPropertyChangedActions<C>(source, propertyName, true);            
        }

        private void RunPropertyChangedDoActions(object source, string propertyName)
        {
            RunPropertyChangedActions<T>(source, propertyName, false);
            RunPropertyChangedActions<C>(source, propertyName, false);
        }

        protected virtual object ListParent { get { throw new NotImplementedException(); } }

        protected void RunPropertyChangedActions<TSource>(object source, string propertyName, bool runSyncActions) where TSource : class
        {
            try
            {
                if (source as TSource == null) return;
                object listener = ListParent;
                if (listener == null) return;
                var pcamType = typeof(PropertyChangedActionManager<,>).MakeGenericType(typeof(TSource), listener.GetType());
                var instance = pcamType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
                if (runSyncActions)
                    pcamType.GetMethod("RunSourceChangedSyncActions").Invoke(instance, new[] { source, listener, propertyName });
                else
                    pcamType.GetMethod("RunSourceChangedDoActions").Invoke(instance, new[] { source, listener, propertyName });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        ////Subclasses should implement this to call RunPropertyChangedActions<TSource, TListener>, specifying TListener and the listener.
        ////The listener is typically the parent object of the list, e.g. SalesOrder is the listener of SalesOrderLineList
        //protected void RunPropertyChangedActions<TSource, TListener>(object source, TListener listener, string propertyName)
        //    where TSource : class
        //    where TListener : class
        //{
        //    if (listener == null || source as TSource == null) return;

        //    PropertyChangedActionManager<TSource, TListener>.Instance.RunSourceChangedActions(source as TSource, listener, propertyName);
        //}

        //This event is fired right after ListChanged is fired.
        //It's done this way so that CurrencyManager can be updated first using ListChanged, and then we can run our own custom logic using AfterListChanged
        //assuming that CurrencyManager is out of date (otherwise it can throw some funky exceptions).
        //We should generally not be subscribing to ListChanged manually anymore.        
        [field: NonSerialized] //Listeners to this may not and shouldn't be serialized.  This matches the behaviour of ListChanged in BindingList.
        public event ListChangedEventHandler AfterListChanged;

        public void OnAfterListChanged(ListChangedEventArgs args)
        {
            AfterListChanged.TryDo(evt => evt(this, args));
        }

        #endregion

    }
}
