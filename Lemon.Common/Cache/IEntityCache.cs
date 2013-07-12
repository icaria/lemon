using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lemon.Common
{
    public interface IEntityCache<out TValue> : IEnumerable<TValue>, IEntityCache
    {
        IEnumerable<TValue> GetDescendingTimestampEnumerator();
        TValue this[int key] { get; }        
    }

    public interface IEntityCache : IEnumerable
    {
        Task InitializeAsync();
        Task IncrementalFetchAsync();
        byte[] LatestTimestamp { get; }
        IDisposable WeakSubscribe<T>(T subscriber, Action<T, DataChangedEventArgsWithSender> onCacheUpdated);
        IDisposable WeakSubscribe(Action<DataChangedEventArgsWithSender> onCacheUpdated);
        Task ReinitializeCache();
    }
}