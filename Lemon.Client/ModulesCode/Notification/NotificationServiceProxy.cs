using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;
using Lemon.Common;
using Winterspring.DataPortal;
using Winterspring.Extensions;

namespace Lemon.Base
{
    public class NotificationServiceProxy : ISubscriptionService
    {
        public void SubscribeAsync(string topic)
        {
            _channel.SubscribeAsync(topic);
        }

        public void UnsubscribeAsync(string topic)
        {
            _channel.UnsubscribeAsync(topic);
        }

        public void Close()
        {
            _channelFactory.Close();
        }

        public event Action<string, object> NotificationReceived;
        public event Action ConnectionFaulted;

        public NotificationServiceProxy()
        {
            var instanceContext = new NotificationCallback();
            instanceContext.OnNotificationReceived += OnNotificationReceived;

            _channelFactory = new DuplexChannelFactory<ISubscriptionService>(instanceContext, String.Format("{0}Notification", ClientConnectionTarget.Instance.EndpointName),
                new EndpointAddress(_endpointAddressUri, new DnsEndpointIdentity("inFlow App Server")));

            _channel = _channelFactory.CreateChannel();

            var clientChannel = (_channel as IClientChannel);
            clientChannel.Faulted += (obj, e) => ConnectionFaulted.TryDo(h => h());
        }

        private void OnNotificationReceived(string topic, PackagedDTO dto)
        {
            var result = dto.UnpackageDTO();
            if (result.Header.Type.Contains("Exception"))
            {
                var innerException = result.Body as Exception;
                throw new InvalidOperationException("Failed receiving notification", innerException);
            }
            NotificationReceived.TryDo(h => h(topic, result.Body));
        }

        private readonly DuplexChannelFactory<ISubscriptionService> _channelFactory;
        private ISubscriptionService _channel;
        private static Uri _endpointAddressUri;
        public static async Task<bool> TestConnectionAsync(string hostName)
        {
            return await Task.Factory.StartNew(() =>
            {
                var origEndpoint = _endpointAddressUri;
                try
                {
                    var dataPortalUriConfig = String.Format("{0}NotificationUri", ClientConnectionTarget.Instance.EndpointName);
                    var url = ConfigurationManager.AppSettings[dataPortalUriConfig];
                    _endpointAddressUri = new Uri(url.Replace("*", hostName));
                    var proxy = new NotificationServiceProxy();
                    var result = (proxy._channel as IClientChannel).Try(channel =>
                    {
                        channel.Open();
                        return channel.State == CommunicationState.Opened;
                    }) ?? false;
                    proxy.Close();
                    return result;
                }
                catch
                {
                    _endpointAddressUri = origEndpoint;
                    return false;
                }
            });
        }
    }
}
