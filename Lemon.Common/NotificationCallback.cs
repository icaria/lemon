using System;
using Archon.Rapid.ProtobufDataPortal;

namespace inFlow.Common.Notification
{
    public class NotificationCallback : INotificationCallback
    {
        public event Action<string, PackagedDTO> OnNotificationReceived;

        void INotificationCallback.NotifyClient(string topic, PackagedDTO dto)
        {
            var handler = OnNotificationReceived;
            if (handler != null)
                handler(topic, dto);
        }
    }
}