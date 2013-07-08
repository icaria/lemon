using System;
using Winterspring.DataPortal;

namespace Lemon.Common
{
    public class NotificationCallback : INotificationCallback
    {
        public event Action<string, PackagedDataTransferObject> OnNotificationReceived;

        void INotificationCallback.NotifyClient(string topic, PackagedDataTransferObject dataTransferObject)
        {
            var handler = OnNotificationReceived;
            if (handler != null)
                handler(topic, dataTransferObject);
        }
    }
}