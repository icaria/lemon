using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.Common
{
    public class MockCache<T> : IEntityCache<T>, IEditableCache<T> where T : class
    {
        private IEnumerable<T> GetEmptyEnumerable() { return new T[] { }; }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEmptyEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new object[] { }.GetEnumerator();
        }

        public async Task ReinitializeCache() 
        { 
            await InitializeAsync(); 
        }

        public Task InitializeAsync()
        {
            return new Task(() => { });
        }

        public Task IncrementalFetchAsync()
        {
            return new Task(() => { });
        }

        public byte[] LatestTimestamp { get; private set; }

        public IDisposable WeakSubscribe<T1>(T1 subscriber, Action<T1, DataChangedEventArgsWithSender> onCacheUpdated)
        {
            return null;
        }

        public IDisposable WeakSubscribe(Action<DataChangedEventArgsWithSender> onCacheUpdated)
        {
            return null;
        }

        public IEnumerable<T> GetDescendingTimestampEnumerator()
        {
            return GetEmptyEnumerable();
        }

        public virtual T this[int key]
        {
            get { return null; }
        }

        public void AddTemporaryValue(T value)
        {
        }
    }
}
