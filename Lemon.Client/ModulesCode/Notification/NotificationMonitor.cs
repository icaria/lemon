using System;
using System.Collections.Generic;
using Winterspring.Extensions;

namespace Lemon.Base
{
    public static class NotificationMonitor
    {
        private static NotificationServiceProxy _proxy;

        private static Dictionary<string, List<Action<string, object>>> _subscriptionMap = new Dictionary<string, List<Action<string, object>>>();

        public static void Start()
        {
            _proxy = new NotificationServiceProxy();
            _proxy.NotificationReceived += RaiseOnNotificationReceived;
            _proxy.ConnectionFaulted += OnConnectionFaulted;
        }

        private static void OnConnectionFaulted()
        {
            //Restart the monitor
            Start();

            //Resubscribe to the things we were already listening to
            foreach (var topic in _subscriptionMap.Keys)
            {
                _proxy.SubscribeAsync(topic);
            }
        }

        public static void Subscribe(string topic, Action<string, object> onNotificationReceived)
        {
            if (onNotificationReceived == null)
                throw new ArgumentNullException("onNotificationReceived");

            topic = topic.ToLower();

            var firstSubscription = !_subscriptionMap.ContainsKey(topic);
            _subscriptionMap.Upsert(topic, onNotificationReceived);

            if (firstSubscription)
            {
                _proxy.SubscribeAsync(topic);
            }
        }

        public static void Unsubscribe(string topic, Action<string, object> onNotificationReceived)
        {
            if (onNotificationReceived == null)
                throw new ArgumentNullException("onNotificationReceived");

            topic = topic.ToLower();

            if (_subscriptionMap.ContainsKey(topic))
            {
                _subscriptionMap[topic].Remove(onNotificationReceived);

                if (_subscriptionMap[topic].Count == 0)
                {
                    _proxy.UnsubscribeAsync(topic);
                }
            }
        }

        public static void Stop()
        {
            _proxy.TryDo(proxy =>
            {
                _proxy.Close();
                _proxy = null;
            });
        }

        private static void RaiseOnNotificationReceived(string topic, object dto)
        {
            topic = topic.ToLower();
            if (_subscriptionMap.ContainsKey(topic))
            {
                _subscriptionMap[topic].ForEach(action => action(topic, dto));
            }
        }
    }
}