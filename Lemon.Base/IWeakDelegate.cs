using System;
using System.Reflection;

namespace Lemon.Base
{
    public interface IWeakDelegate<in T>
    {
        WeakReference Obj { get; }
        MethodInfo Method { get; }
        bool IsAlive { get; }
        void Invoke(T param);
    }

    public interface IWeakDelegate
    {
        WeakReference Obj { get; }
        MethodInfo Method { get; }
        bool IsAlive { get; }
        void Invoke();
    }
}