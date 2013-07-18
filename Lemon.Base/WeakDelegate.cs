using System;
using System.Reflection;
using Winterspring.Extensions;

namespace Lemon.Base
{
    //For a weak delegate with no parameters (i.e. void, since generics in C# don't let you pass in void as a generic type)
    public class WeakDelegate : IWeakDelegate
    {
        public WeakReference Obj { get; private set; }
        public MethodInfo Method { get; private set; }

        public WeakDelegate(object target, MethodInfo method)
        {
            Obj = new WeakReference(target);
            Method = method;
        }

        public void Invoke()
        {
            //Get the target to invoke it on
            object target = Obj.Target;
            //Execute if the target exists (the weak delegate was valid) or it's a static method
            if (Method.IsStatic)
            {
                Method.Invoke(null, null);
            }
            else if (target != null)
            {
                //Don't execute if the object knows that it's already been disposed
                var trackDisposable = target as ITrackDisposable;
                if (trackDisposable.Try(x => !x.IsDisposed) ?? true)
                    Method.Invoke(target, null);
            }
        }

        public bool IsAlive
        {
            get { return Method.IsStatic || (Obj != null && Obj.IsAlive); }
        }
    }

    public class WeakDelegate<T> : IWeakDelegate<T>
    {
        public WeakReference Obj { get; private set; }
        public MethodInfo Method { get; private set; }

        public WeakDelegate(object target, MethodInfo method)
        {
            Obj = new WeakReference(target);
            Method = method;
        }

        public void Invoke(T param)
        {
            //Get the target to invoke it on
            object target = Obj.Target;
            //Execute if the target exists (the weak delegate was valid) or it's a static method
            if (Method.IsStatic)
            {
                Method.Invoke(null, new object[] { param });
            }
            else if (target != null)
            {
                //Don't execute if the object knows that it's already been disposed
                var trackDisposable = target as ITrackDisposable;
                if (trackDisposable.Try(x => !x.IsDisposed) ?? true)
                    Method.Invoke(target, new object[] { param });
            }
        }

        public bool IsAlive
        {
            get { return Method.IsStatic || (Obj != null && Obj.IsAlive); }
        }
    }
}