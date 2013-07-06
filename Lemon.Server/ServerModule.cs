using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Lemon.Common;

namespace Lemon.Server
{
    public class ServerModule : NinjectModule
    {
        public override void Load()
        {                        
            Kernel.Bind<INotificationService, ISubscriptionService, IPublishService>().To<NotificationService>().InSingletonScope();
            //Kernel.Bind(scan => scan.FromAssembliesMatching("Lemon.Server.Sql.dll").SelectAllClasses().InheritedFrom<IEntityCache>().BindAllInterfaces()
            //                        .Configure(x => x.InSingletonScope()));            
        }
    }
}