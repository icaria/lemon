using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Lemon.Base;
using Winterspring.Extensions;
using Csla.Core;
using Csla.Rules;
using Csla;

namespace Lemon.Base
{
    [Serializable()]
    public abstract class WinterspringBusinessBase<TModel> : BusinessBase<TModel> where TModel : WinterspringBusinessBase<TModel>
    {
        /// <summary>
        /// Performs processing required when an automatically calculated
        /// property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property that has changed</param>
        /// <remarks>
        /// This is the same as PropertyHasChanged except that it does not
        /// mark the object as dirty.
        /// </remarks>
        protected void NonPersistentPropertyHasChanged(IPropertyInfo propertyInfo)
        {
            var propertyNames = BusinessRules.CheckRules(propertyInfo);
            OnPropertyChanged(propertyInfo);
        }

        protected ISnapshot<TModel> _snapshot;
        protected void SaveSnapshot() 
        {
            _snapshot = CreateSnapshot();
            _snapshot.TryDo(x => x.Initialize((TModel)this));
        }

        public void InitializeOnClientSide()
        {
            SaveSnapshot();
            CheckAllRules();
        }

        /// <summary>
        /// Like BusinessRules.CheckRules, but it should go recursively through child objects.
        /// </summary>
        public virtual void CheckAllRules()
        {
            BusinessRules.CheckRules();
        }

        protected virtual ISnapshot<TModel> CreateSnapshot() { return null; }

        public static readonly PropertyInfo<bool> HasDataConflictProperty = RegisterProperty<bool>(c => c.HasDataConflict);
        public bool HasDataConflict
        {
            get { return GetProperty(HasDataConflictProperty); }
            set { SetProperty(HasDataConflictProperty, value); }
        }
        
        public virtual string GetHumanReadableName()
        {
            return Util.CodeToHumanReadable(GetType().Name);
        }

        private readonly HashSet<string> _batchedSyncActions = new HashSet<string>();
        private readonly HashSet<string> _batchedDoActions = new HashSet<string>();

        //A List of all the child lists contained by this business object
        protected readonly List<IWinterspringBusinessListBase> ChildLists = new List<IWinterspringBusinessListBase>();

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
                            var prop = _batchedSyncActions.First();
                            _batchedSyncActions.Remove(prop);
                            PropertyChangedActionManager<TModel>.Instance.RunSyncActions((TModel)this, prop);
                        }
                    }
                    _syncMode = value;
                    ChildLists.ForEach(x => x.SyncMode = value);
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
                            var prop = _batchedDoActions.First();
                            _batchedDoActions.Remove(prop);
                            PropertyChangedActionManager<TModel>.Instance.RunDoActions((TModel)this, prop);
                        }
                    }
                    _doMode = value;
                    ChildLists.ForEach(x => x.DoMode = value);
                }
            }
        }

        private void DoPropertyChangedActions(string propertyName)
        {
            DoPropertyChangedSyncActions(propertyName);
            DoPropertyChangedDoActions(propertyName);
        }

        private void DoPropertyChangedDoActions(string propertyName)
        {
            //Run the do commands
            if (DoMode == PropertyChangedActionMode.Batch)
            {
                if (!_batchedDoActions.Contains(propertyName))
                    _batchedDoActions.Add(propertyName);
            }
            else if (DoMode == PropertyChangedActionMode.Normal)
            {
                PropertyChangedActionManager<TModel>.Instance.RunDoActions((TModel) this, propertyName);
            }
            else if (DoMode == PropertyChangedActionMode.Suppress)
            {
                //If it's suppressed, just ignore the change
            }
        }

        private void DoPropertyChangedSyncActions(string propertyName)
        {
            //Run the sync commands
            if (SyncMode == PropertyChangedActionMode.Batch)
            {
                if (!_batchedSyncActions.Contains(propertyName))
                    _batchedSyncActions.Add(propertyName);
            }
            else if (SyncMode == PropertyChangedActionMode.Normal)
            {
                PropertyChangedActionManager<TModel>.Instance.RunSyncActions((TModel) this, propertyName);
            }
            else if (SyncMode == PropertyChangedActionMode.Suppress)
            {
                //If it's suppressed, just ignore the change
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            DoPropertyChangedActions(propertyName);
            base.OnPropertyChanged(propertyName);
        }

        //Use this one for CSLA managed properties
        protected void SetPropertyDontMarkDirty(IPropertyInfo property, object value)
        {
            LoadProperty(property, value);
            BusinessRules.CheckRules(property);
            OnPropertyChanged(property);
        }

        //Use this one for private fields (where CSLA doesn't manage the property)
        protected void SetPropertyDontMarkDirty<T>(PropertyInfo<T> propertyInfo, ref T field, T newValue)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                BusinessRules.CheckRules(propertyInfo);
                OnPropertyChanged(propertyInfo);
            }
        }

        public CslaMetadata CslaMetadata
        {
            get
            {
                return new CslaMetadata { IsDirty = this.IsDirty, IsNew = this.IsNew };
            }
            set
            {
                if (this.IsNew != value.IsNew)
                {
                    if (value.IsNew)
                        MarkNew();
                    else
                        MarkOld();
                }

                if (this.IsDirty != value.IsDirty)
                {
                    if (value.IsDirty)
                        MarkDirty(true);
                    else
                        MarkClean();
                }
            }
        }
    }
}
