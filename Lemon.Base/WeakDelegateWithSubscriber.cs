using System;
using System.Reflection;
using Winterspring.Extensions;

namespace Lemon.Base
{
    public class WeakDelegateWithSubscriber<TSubscriber> : IWeakDelegate
    {
        public WeakReference Obj { get; private set; }
        public MethodInfo Method { get; private set; }

        public WeakDelegateWithSubscriber(TSubscriber subscriber, Action<TSubscriber> action)
        {
            Obj = new WeakReference(subscriber);
            Method = action.Method;
        }

        public void Invoke()
        {
            //Get the target to invoke it on
            object target = Obj.Target;
            //Execute if the target exists (the weak delegate was valid) or it's a static method
            if (Method.IsStatic)
            {
                Method.Invoke(null, new[] { target });
            }
            else if (target != null)
            {
                //Don't execute if the object knows that it's already been disposed
                var trackDisposable = target as ITrackDisposable;
                if (trackDisposable.Try(x => !x.IsDisposed) ?? true)
                    Method.Invoke(target, new[] { target });
            }
        }

        public bool IsAlive
        {
            get { return Method.IsStatic || (Obj.Try(x => x.IsAlive) ?? false); }
        }
    }

    public class WeakDelegateWithSubscriber<T, TSubscriber> : IWeakDelegate<T>
    {
        public WeakReference Obj { get; private set; }
        public MethodInfo Method { get; private set; }

        public WeakDelegateWithSubscriber(TSubscriber subscriber, Action<TSubscriber, T> action)
        {
            Obj = new WeakReference(subscriber);
            Method = action.Method;
        }

        public void Invoke(T param)
        {
            //Get the target to invoke it on
            object target = Obj.Target;
            //Execute if the target exists (the weak delegate was valid) or it's a static method
            if (Method.IsStatic)
            {
                Method.Invoke(null, new[] { target, param });
            }
            else if (target != null)
            {
                //Don't execute if the object knows that it's already been disposed
                var trackDisposable = target as ITrackDisposable;
                if (trackDisposable.Try(x => x.IsDisposed) ?? true)
                    Method.Invoke(target, new[] { target, param });
            }
        }

        public bool IsAlive
        {
            get { return Method.IsStatic || (Obj.Try(x => x.IsAlive) ?? false); }
        }
    }
}