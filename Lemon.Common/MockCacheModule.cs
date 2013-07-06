using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Lemon.Common.Cache.Mock;

namespace Lemon.Common
{
    public class MockCacheModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(scan => scan.FromThisAssembly()
                                    .SelectAllClasses()
                                    .InheritedFrom<IEntityCache>()
                                    .InNamespaceOf<MockAccountCache>()
                                    .BindAllInterfaces()
                                    .Configure(x => x.InSingletonScope()));
        }
    }
}