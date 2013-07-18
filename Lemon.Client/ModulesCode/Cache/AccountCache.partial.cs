using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Common;

namespace Lemon.Client.Cache
{
    public partial class AccountCache : ClientEntityCache<AccountCacheObject>, IAccountCache
    {
        protected override string ObservingEntityName { get { return "CacheUpdated.BASE_TaxCode"; } }
    }
}
