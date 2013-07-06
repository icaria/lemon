using System.ServiceModel;
using Winterspring.Lemon.DataPortal;

namespace Lemon.Common
{
    public interface INotificationCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyClient(string topic, PackagedDTO dto);
    }
}