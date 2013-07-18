using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Linq;
using Winterspring.Extensions;

namespace Lemon.Base
{
    public class WeakSubscription : IDisposable
    {
        private Action _onDispose;
        public WeakSubscription(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose.TryDo(h => h());
        }
    }


    //With ideas from http://diditwith.net/2007/03/23/SolvingTheProblemWithEventsWeakEventHandlers.aspx
    //for some reason it needs to be serializable, or else Company Settings won't work
    public class WeakEvent<T>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected List<IWeakDelegate<T>> EventSubscribers = new List<IWeakDelegate<T>>();

        public IDisposable AddHandler<TSubscriber>(TSubscriber subscriber, Action<TSubscriber, T> action)
        {
            if (!action.Method.IsStatic && action.Target != (object)subscriber)
                throw new ArgumentException("The action's target must be on the subscriber, or the action must be static (no access to outside variables)", "onCacheUpdated");

            var weakDelegate = new WeakDelegateWithSubscriber<T, TSubscriber>(subscriber, action);
            EventSubscribers.Add(weakDelegate);
            return new WeakSubscription(() => EventSubscribers.Remove(weakDelegate));
        }

        //You can use this method for simplicity, but it doesn't have anything that control's the subscriptions lifespan.         
        public IDisposable AddHandler(Action<T> handler)
        {
            var weakDelegate = new WeakDelegate<T>(handler.Target, handler.Method);
            EventSubscribers.Add(weakDelegate);
            return new WeakSubscription(() => EventSubscribers.Remove(weakDelegate));
        }

        public void RemoveHandler(object o, MethodInfo method)
        {
            foreach (var d in EventSubscribers)
            {
                var r = d.Obj;
                var target = r.Target;
                if (target == o && d.Method == method)
                {
                    EventSubscribers.Remove(d);
                    return;
                }
            }
        }

        public void RemoveHandler(Action<T> action)
        {
            RemoveHandler(action.Target, action.Method);
        }

        public virtual void Invoke(T param)
        {
            if (EventSubscribers == null) return;

            lock (EventSubscribers)
            {
                PurgeList();

                //Due to race conditions, even after purging these might not be valid
                //If we just Invoke the delegate directly from obj, it will lead to 
                // "System.InvalidOperationException: Collection was modified; enumeration operation may not execute."
                // Instead, do a shallow copy and invoke the copy instead
                var eventSubscribersCopy = EventSubscribers.ToArray();

                foreach (var d in eventSubscribersCopy)
                {
                    //Try to invoke the update and just log any errors.
                    try
                    {
                        d.Invoke(param);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        // Purge List - Remove any references to objects that have already expired from our list of objects
        private void PurgeList()
        {
            foreach (var toRemove in EventSubscribers.Where(x => !x.IsAlive).ToArray())
                EventSubscribers.Remove(toRemove);
        }
    }

    //Same as above, but the non-generic type (so that you can use void)
    public class WeakEvent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected List<IWeakDelegate> EventSubscribers = new List<IWeakDelegate>();

        public IDisposable AddHandler<TSubscriber>(TSubscriber subscriber, Action<TSubscriber> action)
        {
            if (!action.Method.IsStatic && action.Target != (object)subscriber)
                throw new ArgumentException("The action's target must be on the subscriber, or the action must be static (no access to outside variables)", "onCacheUpdated");

            var weakDelegate = new WeakDelegateWithSubscriber<TSubscriber>(subscriber, action);
            EventSubscribers.Add(weakDelegate);
            return new WeakSubscription(() => EventSubscribers.Remove(weakDelegate));
        }

        //You can use this method for simplicity, but it doesn't have anything that control's the subscriptions lifespan.         
        public IDisposable AddHandler(Action handler)
        {
            var weakDelegate = new WeakDelegate(handler.Target, handler.Method);
            EventSubscribers.Add(weakDelegate);
            return new WeakSubscription(() => EventSubscribers.Remove(weakDelegate));
        }

        public void RemoveHandler(object o, MethodInfo method)
        {
            if (EventSubscribers == null) return;

            foreach (WeakDelegate d in EventSubscribers)
            {
                WeakReference r = d.Obj;
                object target = r.Target;
                if (target == o && d.Method == method)
                {
                    EventSubscribers.Remove(d);
                    return;
                }
            }
        }

        public void RemoveHandler(Action action)
        {
            RemoveHandler(action.Target, action.Method);
        }

        public virtual void Invoke()
        {
            if (EventSubscribers == null) return;

            lock (EventSubscribers)
            {
                PurgeList();

                //Due to race conditions, even after purging these might not be valid
                //If we just Invoke the delegate directly from obj, it will lead to 
                // "System.InvalidOperationException: Collection was modified; enumeration operation may not execute."
                // Instead, do a shallow copy and invoke the copy instead
                var eventSubscribersCopy = EventSubscribers.ToArray();

                foreach (var d in eventSubscribersCopy)
                {
                    //Try to invoke the update and just log any errors.
                    try
                    {
                        d.Invoke();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        // Purge List - Remove any references to objects that have already expired from our list of objects
        private void PurgeList()
        {
            foreach (var toRemove in EventSubscribers.Where(x => !x.IsAlive).ToArray())
                EventSubscribers.Remove(toRemove);
        }
    }
}
