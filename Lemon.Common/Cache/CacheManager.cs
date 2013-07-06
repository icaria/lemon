using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Winterspring.Extensions;
//using Ninject;

namespace Lemon.Common
{
    public class CacheManager
    {
        public List<IEntityCache> Caches { get; private set; }

        public CacheManager(List<IEntityCache> caches)
        {
            Caches = caches;
        }

        public async Task InitializeAsync()
        {
            await Task.Factory.WhenAll(Caches.Select(x => x.InitializeAsync()).ToArray());
        }
    }
}
