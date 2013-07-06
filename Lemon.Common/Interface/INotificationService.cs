using System.ServiceModel;
using Winterspring.Lemon.DataPortal;

namespace Lemon.Common
{
    [ServiceContract(CallbackContract = (typeof(INotificationCallback)), Namespace = "http://inFlow")]
    public interface INotificationService : ISubscriptionService, IPublishService
    {        
    }

    [ServiceContract(CallbackContract = (typeof(INotificationCallback)), Namespace="http://inFlow")]
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