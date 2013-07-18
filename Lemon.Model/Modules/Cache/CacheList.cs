using System;
using System.Linq;
using Csla.Core;
using Lemon.Base;
using Lemon.Common;
using Winterspring.DataPortal;

namespace Lemon.Model
{
    [DataTransferObject]
    public class CacheList<TValue> : MobileList<TValue>
        where TValue : IObjectWithTimestamp
    {
        protected virtual void DataPortal_Fetch(byte[] latestTimestamp = null)
        {
            //The server side enumerator should be Ordered by Timestamp Descending order
            var list = ServiceLocator.Get<IEntityCache<TValue>>().GetDescendingTimestampEnumerator().
                TakeWhile(x => latestTimestamp == null || new NaturalOrderByteArrayComparer().Compare(x.Timestamp, latestTimestamp) > 0).Reverse();
            AddRange(list);
        }
    }
}