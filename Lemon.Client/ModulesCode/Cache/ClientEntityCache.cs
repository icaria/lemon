using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Csla;
using Lemon.Base;
using Lemon.Common;
using Lemon.Model;

namespace Lemon.Client.Cache
{
    public abstract class ClientEntityCache<TValue> : EntityCache<TValue>
        where TValue : class, IObjectWithTimestamp, IObjectWithId
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            NotificationMonitor.Subscribe(ObservingEntityName, OnNotificationReceived);
        }

        private void OnNotificationReceived(string topic, object body)
        {
            if (ObservingEntityName.Equals(topic, StringComparison.InvariantCultureIgnoreCase))
            {
                var eventArgs = body as DataChangedEventArgs;
                if (eventArgs == null)
                    throw new Exception("Cannot get data changed event args");

                DoDataSourceChanged(eventArgs);
            }
        }

        protected override async Task<IList<TValue>> DoIncrementalFetchAsync()
        {
            return LatestTimestamp == null ? (await DataPortal.FetchAsync<CacheList<TValue>>())
                       : (await DataPortal.FetchAsync<CacheList<TValue>>(LatestTimestamp));
        }
    }
}