using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Winterspring.Extensions;
using Winterspring.Lemon.DataPortal;

namespace Lemon.Common
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NotificationService : INotificationService
    {        
        private readonly Dictionary<string, List<INotificationCallback>> _subscriberMap = new Dictionary<string, List<INotificationCallback>>();

        public void SubscribeAsync(string topic)
        {
            //Let's just always use lower case to make it easier
            topic = topic.ToLower();
            var client = OperationContext.Current.GetCallbackChannel<INotificationCallback>();
            if (!_subscriberMap.ContainsKey(topic))
            {
                _subscriberMap.Add(topic, new List<INotificationCallback>());                
            }

            //Don't add multiple references to the same client
            if (!_subscriberMap[topic].Contains(client))
            {
                _subscriberMap[topic].Add(client);
            }
        }

        public void UnsubscribeAsync(string topic)
        {
            //Let's just always use lower case to make it easier
            topic = topic.ToLower();
            var client = OperationContext.Current.GetCallbackChannel<INotificationCallback>();
            if (_subscriberMap.ContainsKey(topic))
            {
                _subscriberMap[topic].Remove(client);
            }
        }

        public void BroadcastAsync(string topic, PackagedDTO dto)
        {
            //Let's just always use lower case to make it easier
            topic = topic.ToLower();
            if (_subscriberMap.ContainsKey(topic))
            {
                //Clean up all the clients that might have been closed somehow
                var nonOpenedClients = _subscriberMap[topic].Cast<IClientChannel>().Where(x => x.State != CommunicationState.Opened).ToArray();
                nonOpenedClients.Cast<INotificationCallback>().ForEach(x => _subscriberMap[topic].Remove(x));                

                foreach (var client in _subscriberMap[topic])
                {                    
                    client.NotifyClient(topic, dto);
                }
            }
        }
    }
}