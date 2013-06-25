using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Configuration;
using System.Threading.Tasks;
using Lemon.LanguageExtensions;
using Lemon.Base;
//using Lemon.DataPortal;
using Ninject;
//using Lemon.Common;
//using Lemon.Common.Notification;
//using Lemon.Server.SQL;
using log4net;
using System.ServiceModel.Discovery;

namespace Lemon.Server
{
    public class LemonAppServer
    {
//        private ServiceHost _netService;
//        private ServiceHost _notificationService;
//        private ILog _log = LogManager.GetLogger("inFlow");

//        private CacheManager _cacheManager;
//        private Func<INotificationService> _notificationServiceFactory;

//        public LemonAppServer()
//        {
//            WireUpCompositionMap();
//        }

//        private void WireUpCompositionMap()
//        {
//            var kernel = new StandardKernel(new ServerModule());
//            kernel.Bind<Func<INotificationService>>().ToMethod(context => () => context.Kernel.Get<INotificationService>());

//            ServiceLocator.Initialize(kernel);
//            _cacheManager = ServiceLocator.Get<CacheManager>();
//            _notificationServiceFactory = ServiceLocator.Get<Func<INotificationService>>();
//        }

//        public void Start(string databaseProfile)
//        {
//            if (_netService != null)
//                _netService.Close();

//            try
//            {
//                _log = LogManager.GetLogger(databaseProfile.ToUpper());

//                Initialize(databaseProfile);

//                var dataPortalUriConfig = String.Format("{0}DataPortalUri", databaseProfile);
//                var notificationUriConfig = String.Format("{0}NotificationUri", databaseProfile);
//                var dataPortalUri = ConfigurationManager.AppSettings[dataPortalUriConfig];
//                var notificationUri = ConfigurationManager.AppSettings[notificationUriConfig];

//                var hostName = Dns.GetHostName();
//                dataPortalUri = dataPortalUri.Replace("*", hostName);
//                notificationUri = notificationUri.Replace("*", hostName);

//                _netService = new ServiceHost(typeof(ProtobufWcfService), new[] { new Uri(dataPortalUri) });
//                _netService.Description.Endpoints.FirstOrDefault(x => x.Contract.ContractType == typeof(IProtobufWcfService)).TryDo(
//                    endpoint =>
//                    {
//                        var behavior = new EndpointDiscoveryBehavior();
//                        behavior.Scopes.Add(new Uri(String.Format("http://{0}", databaseProfile)));
//                        endpoint.Behaviors.Add(behavior);
//                    });
//                _netService.Open();

//                _notificationService = new ServiceHost(_notificationServiceFactory(), new[] { new Uri(notificationUri) });
//                _notificationService.Description.Endpoints.FirstOrDefault(x => x.Contract.ContractType == typeof(ISubscriptionService)).TryDo(
//                    endpoint =>
//                    {
//                        var behavior = new EndpointDiscoveryBehavior();
//                        behavior.Scopes.Add(new Uri(ServerConnectionTarget.Instance.IsSampleDatabase ? "http://inFlowSample" : "http://inFlow"));
//                        endpoint.Behaviors.Add(behavior);
//                    });
//                _notificationService.Open();

//                Task.Factory.StartNew(async () => await PostInitializeAsync()).Wait();
//            }
//            catch (Exception ex)
//            {
//                _log.Info(ex.Message, ex);

//#if DEBUG
//                throw;
//#endif
//            }
//        }

//        public void Stop()
//        {
//            try
//            {
//                if (_netService != null)
//                {
//                    _netService.Close();
//                    _netService = null;
//                }

//                if (_notificationService != null)
//                {
//                    _notificationService.Close();
//                    _notificationService = null;
//                }
//            }
//            catch (Exception ex)
//            {
//                _log.Info(ex.Message, ex);
//            }
//        }

//        private void Initialize(string databaseProfile)
//        {
//            ProtoBufInitializer.Initialize();

//            ServerConnectionTarget.Initialize(new ServerConnectionTarget(databaseProfile));
//        }

//        private async Task PostInitializeAsync()
//        {
//            SqlDataMonitor.Start(_log);

//            await _cacheManager.InitializeAsync();
//            _cacheManager.Caches.Cast<IEntityCache<object>>().ForEach(cache =>
//                {
//                    _log.InfoFormat("{0} has {1} items", cache.GetType().Name, cache.Count());
//                    foreach (var item in cache)
//                    {
//                        _log.Info(item);
//                    }
//                });

//            _log.Info("inFlow App Server Initialized");
//        }
    }
}
