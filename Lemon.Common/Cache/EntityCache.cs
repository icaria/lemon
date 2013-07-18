using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Lemon.Base;
using Winterspring.Extensions;

namespace Lemon.Common
{
    public abstract class EntityCache<TValue> : IEntityCache<TValue>, IEditableCache<TValue>
        where TValue : class, IObjectWithTimestamp, IObjectWithId
    {
        public static NaturalOrderByteArrayComparer TimestampComparer = new NaturalOrderByteArrayComparer();
        public byte[] LatestTimestamp { get { return _cacheLinkedList.Last.Try(lastNode => lastNode.Value.Timestamp); } }

        private readonly WeakEvent<DataChangedEventArgsWithSender> _cacheUpdatedEvent = new WeakEvent<DataChangedEventArgsWithSender>();
        public IDisposable WeakSubscribe<T>(T subscriber, Action<T, DataChangedEventArgsWithSender> onCacheUpdated)
        {
            return _cacheUpdatedEvent.AddHandler(subscriber, onCacheUpdated);
        }

        public IDisposable WeakSubscribe(Action<DataChangedEventArgsWithSender> onCacheUpdated)
        {
            return _cacheUpdatedEvent.AddHandler(onCacheUpdated);
        }

        protected abstract string ObservingEntityName { get; }

        //This is the linked list that actually stores the cache value
        private readonly LinkedList<TValue> _cacheLinkedList = new LinkedList<TValue>();
        private readonly Dictionary<int, LinkedListNode<TValue>> _cacheMap = new Dictionary<int, LinkedListNode<TValue>>();
        protected ReaderWriterLock Lock = new ReaderWriterLock();

        /// <summary>
        /// Wipe out the entire cache so that next time it'll be freshly initialized.
        /// </summary>
        public async Task ReinitializeCache()
        {
            ClearCache();
            await InitializeAsync();
        }

        private void ClearCache()
        {
            _cacheLinkedList.Clear();
            _cacheMap.Clear();
        }

        public async Task IncrementalFetchAsync()
        {
            //The assumption is that IncrementalFetchAsync will return the cache objects
            //sorted ascendingly
            var results = await DoIncrementalFetchAsync();
            if (results.Any())
            {
                Lock.AcquireWriterLock(-1);
                try
                {
                    foreach (var item in results)
                    {
                        //Remove the older node if it got updated
                        if (_cacheMap.ContainsKey(item.Id))
                        {
                            _cacheLinkedList.Remove(_cacheMap[item.Id]);
                        }
                        _cacheMap.Upsert(item.Id, _cacheLinkedList.AddLast(item));
                    }
                }
                finally
                {
                    Lock.ReleaseWriterLock();
                }
                //After we update our cache, we should let our subscribers know
                //that their cache or whatever they were displaying is no longer valid                
                RaiseOnCacheUpdated(new DataChangedEventArgs(DataChangeOperation.Update, ObservingEntityName));
            }
        }

        protected abstract Task<IList<TValue>> DoIncrementalFetchAsync();
        protected Func<TValue, int> KeySelector { get { return value => value.Id; } }

        protected virtual void RaiseOnCacheUpdated(DataChangedEventArgs e)
        {
            _cacheUpdatedEvent.Invoke(new DataChangedEventArgsWithSender(this, e.Operation, e.TableName, e.DeletedIds));
        }

        protected async void DoDataSourceChanged(DataChangedEventArgs e)
        {
            if (e.Operation == DataChangeOperation.Delete)
            {
                PurgeEntries(e.DeletedIds);
                RaiseOnCacheUpdated(e);
            }
            else
            {
                await IncrementalFetchAsync();
            }
        }

        protected void PurgeEntries(List<int> deletedIds)
        {
            Lock.AcquireWriterLock(-1);
            try
            {
                deletedIds.ForEach(deletedId =>
                {
                    if (_cacheMap.ContainsKey(deletedId))
                    {
                        _cacheLinkedList.Remove(_cacheMap[deletedId]);
                        _cacheMap.Remove(deletedId);
                    }
                });
            }
            finally
            {
                Lock.ReleaseWriterLock();
            }
        }

        public TValue Get(int key)
        {
            Lock.AcquireReaderLock(-1);
            try
            {
                return _cacheMap.ContainsKey(key) ? _cacheMap[key].Value : null;
            }
            finally
            {
                Lock.ReleaseReaderLock();
            }
        }

        public TValue this[int key]
        {
            get { return Get(key); }
        }

        public void AddTemporaryValue(TValue item)
        {
            Lock.AcquireWriterLock(-1);
            try
            {
                //Only add the temp value if the timestamp is newer than what we have in the cache already or if it doesn't exist
                if (_cacheMap.ContainsKey(item.Id) && TimestampComparer.Compare(_cacheMap[item.Id].Value.Timestamp, item.Timestamp) < 0)
                {
                    _cacheLinkedList.Remove(_cacheMap[item.Id]);
                    _cacheMap.Upsert(item.Id, _cacheLinkedList.AddLast(item));
                }
                else if (!_cacheMap.ContainsKey(item.Id))
                {
                    _cacheMap.Upsert(item.Id, _cacheLinkedList.AddLast(item));
                }

                //We need to let people know that the cache has been updated, because the incremental fetch most likely won't do anything
                //because the timestamp is already up to date
                RaiseOnCacheUpdated(new DataChangedEventArgs(DataChangeOperation.Insert, ObservingEntityName));
            }
            finally
            {
                Lock.ReleaseWriterLock();
            }
        }

        public IEnumerable<TValue> GetDescendingTimestampEnumerator()
        {
            Lock.AcquireReaderLock(-1);
            try
            {
                var node = _cacheLinkedList.Last;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Previous;
                }
            }
            finally
            {
                Lock.ReleaseReaderLock();
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            Lock.AcquireReaderLock(-1);
            try
            {
                return _cacheLinkedList.GetEnumerator();
            }
            finally
            {
                Lock.ReleaseReaderLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async virtual Task InitializeAsync()
        {
            await IncrementalFetchAsync();
        }
    }
}