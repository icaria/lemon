using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterspring.Extensions;
using Csla.Core;
using System.Reflection;

namespace Lemon.Base
{
    //NOTE:
    //There are two copies of this class:
    //- one with one generic parameter for a class listening to PropertyChanged events from itself
    //- another with two generic parameters for a class listening to PropertyChanged events from itself and another source object
    //Most likely any changes you make to one class will need to be made to the other one as well.

    public enum ActionsToPerform
    {
        ImmediateOnly,
        BatchedOnly,
        ImmediateAndBatched,
    }

    #region Class with two generic type parameters


    /// <summary>
    /// This class is used to make it easier to update actions based on PropertyChangedNotifications
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TListener"></typeparam>
    public class PropertyChangedActionManager<TSource, TListener>
    {
        private Dictionary<string, List<Action<TSource, TListener>>> sourceChangedSyncActions = new Dictionary<string, List<Action<TSource, TListener>>>();
        private Dictionary<string, List<Action<TSource, TListener>>> listenerChangedSyncActions = new Dictionary<string, List<Action<TSource, TListener>>>();
        private Dictionary<string, List<Action<TSource, TListener>>> sourceChangedDoActions = new Dictionary<string, List<Action<TSource, TListener>>>();
        private Dictionary<string, List<Action<TSource, TListener>>> listenerChangedDoActions = new Dictionary<string, List<Action<TSource, TListener>>>();

        private PropertyChangedActionManager() { }

        private static readonly PropertyChangedActionManager<TSource, TListener> _Instance = new PropertyChangedActionManager<TSource, TListener>();
        public static PropertyChangedActionManager<TSource, TListener> Instance { get { return _Instance; } }

        public void RegisterSourceChangedSyncAction(Action<TSource, TListener> action, params string[] updateFrom)
        {
            RegisterActionHelper(sourceChangedSyncActions, action, updateFrom);
        }

        public void RegisterListenerChangedSyncAction(Action<TSource, TListener> action, params string[] updateFrom)
        {
            RegisterActionHelper(listenerChangedSyncActions, action, updateFrom);
        }

        public void RegisterSourceChangedDoAction(Action<TSource, TListener> action, params string[] updateFrom)
        {
            RegisterActionHelper(sourceChangedDoActions, action, updateFrom);
        }

        public void RegisterListenerChangedDoAction(Action<TSource, TListener> action, params string[] updateFrom)
        {
            RegisterActionHelper(listenerChangedDoActions, action, updateFrom);
        }

        private static void RegisterActionHelper(Dictionary<string, List<Action<TSource, TListener>>> actionSet, Action<TSource, TListener> action, params string[] updateFrom)
        {
            foreach (var p in updateFrom)
            {
                if (!actionSet.ContainsKey(p))
                    actionSet[p] = new List<Action<TSource, TListener>>();
                actionSet[p].Add(action);
            }
        }

        public OnChangeCommandObject OnChange(params string[] updateFrom)
        {
            return new OnChangeCommandObject(this, updateFrom);
        }

        public OnChangeCommandObject OnChange(params IPropertyInfo[] updateFrom)
        {
            return new OnChangeCommandObject(this, updateFrom.Select(x => x.Name).ToArray());
        }

        public OnChangeCommandObject OnChange(params ChangingPropertyInfo[] updateFrom)
        {
            return new OnChangeCommandObject(this, updateFrom.Select(x => x.Name).ToArray());
        }

        public class OnChangeCommandObject
        {
            public OnChangeCommandObject(PropertyChangedActionManager<TSource, TListener> manager, string[] updateFrom)
            {
                this.manager = manager;
                this.UpdateFrom = updateFrom;
            }

            private PropertyChangedActionManager<TSource, TListener> manager;
            public string[] UpdateFrom { get; set; }

            public OnChangeCommandObject Sync(Action<TSource, TListener> action)
            {
                manager.RegisterSourceChangedSyncAction(action, UpdateFrom);
                return this;
            }

            public OnChangeCommandObject Do(Action<TSource, TListener> action)
            {
                manager.RegisterSourceChangedDoAction(action, UpdateFrom);
                return this;
            }

            public OnChangeCommandObject OnChange(params string[] newUpdateFrom)
            {
                return new OnChangeCommandObject(manager, newUpdateFrom);
            }

            public OnChangeCommandObject OnChange(params IPropertyInfo[] newUpdateFrom)
            {
                return new OnChangeCommandObject(manager, newUpdateFrom.Select(x => x.Name).ToArray());
            }

            public OnChangeCommandObject OnChange(params ChangingPropertyInfo[] newUpdateFrom)
            {
                return new OnChangeCommandObject(manager, newUpdateFrom.Select(x => x.Name).ToArray());
            }
        }

        public void RunSourceChangedActions(TSource source, TListener listener, string changedPropertyName = null)
        {
            RunActionsHelper(source, listener, new[] { sourceChangedSyncActions, sourceChangedDoActions }, changedPropertyName);
        }

        public void RunSourceChangedSyncActions(TSource source, TListener listener, string changedPropertyName = null)
        {
            RunActionsHelper(source, listener, new[] { sourceChangedSyncActions, }, changedPropertyName);
        }

        public void RunSourceChangedDoActions(TSource source, TListener listener, string changedPropertyName = null)
        {
            RunActionsHelper(source, listener, new[] { sourceChangedDoActions, }, changedPropertyName);
        }

        public void RunListenerChangedActions(TSource source, TListener listener, string changedPropertyName = null)
        {
            RunActionsHelper(source, listener, new[] { listenerChangedSyncActions, listenerChangedDoActions }, changedPropertyName);
        }

        public void RunSyncActions(TSource source, TListener listener, string changedPropertyName = null)
        {
            RunActionsHelper(source, listener, new[] { sourceChangedSyncActions, listenerChangedSyncActions }, changedPropertyName);
        }

        private static void RunActionsHelper(TSource source, TListener listener, IEnumerable<Dictionary<string, List<Action<TSource, TListener>>>> actionSets, string changedPropertyName = null)
        {
            if (source == null || listener == null)
                return;

            if (string.IsNullOrEmpty(changedPropertyName))
            {
                //Run all the actions on all the actions sets
                foreach (var a in actionSets.SelectMany(x => x.Values).SelectMany(x => x))
                    a(source, listener);
            }
            else
            {
                //Select this property on all the actions sets, then run all those actions
                foreach (var a in actionSets.Select(x => x.GetWithDefault(changedPropertyName, new List<Action<TSource, TListener>>())).SelectMany(x => x))
                    a(source, listener);
            }
        }
    }

    #endregion






    #region Class with one generic type parameter

    /// <summary>
    /// This class is used to make it easier to update actions based on PropertyChangedNotifications
    /// </summary>
    /// <typeparam name="TChanging"></typeparam>
    /// <typeparam name="TListener"></typeparam>
    public class PropertyChangedActionManager<TListener>
    {
        private Dictionary<string, List<Action<TListener>>> _syncActions = new Dictionary<string, List<Action<TListener>>>();
        private Dictionary<string, List<Action<TListener>>> _doActions = new Dictionary<string, List<Action<TListener>>>();

        private static readonly PropertyChangedActionManager<TListener> _Instance = new PropertyChangedActionManager<TListener>();

        private PropertyChangedActionManager()
        {
        }

        public static PropertyChangedActionManager<TListener> Instance
        {
            get { return _Instance; }
        }

        public void RegisterSyncAction(Action<TListener> action, params string[] updateFrom)
        {
            RegisterActionHelper(_syncActions, action, updateFrom);
        }

        public void RegisterDoAction(Action<TListener> action, params string[] updateFrom)
        {
            RegisterActionHelper(_doActions, action, updateFrom);
        }

        private static void RegisterActionHelper(Dictionary<string, List<Action<TListener>>> actionSet, Action<TListener> action, params string[] updateFrom)
        {
            foreach (var p in updateFrom)
            {
                if (!actionSet.ContainsKey(p))
                    actionSet[p] = new List<Action<TListener>>();
                actionSet[p].Add(action);
            }
        }

        public OnChangeCommandObject OnChange(params string[] updateFrom)
        {
            return new OnChangeCommandObject(this, updateFrom);
        }

        public OnChangeCommandObject OnChange(params IPropertyInfo[] updateFrom)
        {
            return new OnChangeCommandObject(this, updateFrom.Select(x => x.Name).ToArray());
        }

        public OnChangeCommandObject OnChange(params ChangingPropertyInfo[] updateFrom)
        {
            return new OnChangeCommandObject(this, updateFrom.Select(x => x.Name).ToArray());
        }

        public void RunSyncActions(TListener listener)
        {
            RunActionsHelper(listener, _syncActions);
        }

        public void RunDoActions(TListener listener)
        {
            RunActionsHelper(listener, _doActions);
        }

        public void RunActions(TListener listening, string changedPropertyName)
        {
            RunSyncActions(listening, changedPropertyName);
            RunDoActions(listening, changedPropertyName);
        }

        public void RunSyncActions(TListener listening, string changedPropertyName)
        {
            RunActionsHelper(listening, changedPropertyName, _syncActions);
        }

        public void RunDoActions(TListener listening, string changedPropertyName)
        {
            RunActionsHelper(listening, changedPropertyName, _doActions);
        }

        public class OnChangeCommandObject
        {
            public OnChangeCommandObject(PropertyChangedActionManager<TListener> manager, string[] updateFrom)
            {
                this.manager = manager;
                this.UpdateFrom = updateFrom;
            }

            private PropertyChangedActionManager<TListener> manager;
            public string[] UpdateFrom { get; set; }

            public OnChangeCommandObject Sync(Action<TListener> action)
            {
                manager.RegisterSyncAction(action, UpdateFrom);
                return this;
            }

            public OnChangeCommandObject Do(Action<TListener> action)
            {
                manager.RegisterDoAction(action, UpdateFrom);
                return this;
            }

            public OnChangeCommandObject OnChange(params string[] newUpdateFrom)
            {
                return new OnChangeCommandObject(manager, newUpdateFrom);
            }

            public OnChangeCommandObject OnChange(params IPropertyInfo[] newUpdateFrom)
            {
                return new OnChangeCommandObject(manager, newUpdateFrom.Select(x => x.Name).ToArray());
            }

            public OnChangeCommandObject OnChange(params ChangingPropertyInfo[] newUpdateFrom)
            {
                return new OnChangeCommandObject(manager, newUpdateFrom.Select(x => x.Name).ToArray());
            }
        }

        private static void RunActionsHelper(TListener listener, Dictionary<string, List<Action<TListener>>> actionSet)
        {
            foreach (var actionList in actionSet.Values)
                foreach (var action in actionList)
                    action(listener);
        }

        private static void RunActionsHelper(TListener listening, string changedPropertyName, Dictionary<string, List<Action<TListener>>> actionSet)
        {
            if (listening != null)
            {
                actionSet.Keys.FirstOrDefault(x => x == changedPropertyName).TryDo(x =>
                {
                    foreach (var a in actionSet[x]) { a(listening); }
                });
            }
        }
    }

    #endregion

}
