using System.ServiceModel;
using Winterspring.DataPortal;

namespace Lemon.Common
{
    [ServiceContract(CallbackContract = (typeof(INotificationCallback)), Namespace = "http://Lemon")]
    public interface INotificationService : ISubscriptionService, IPublishService
    {        
    }

    [ServiceContract(CallbackContract = (typeof(INotificationCallback)), Namespace="http://Lemon")]
    public interface ISubscriptionService
    {
        [OperationContract(IsOneWay = true)]
        void SubscribeAsync(string topic);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeAsync(string topic);
    }

    [ServiceContract(Namespace="http://inFlow")]
    public interface IPublishService
    {
        [OperationContract(IsOneWay = true)]
        void BroadcastAsync(string topic, PackagedDTO dto);
    }
}