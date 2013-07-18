using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Winterspring.DataPortal;

namespace Lemon.Common
{
    [ServiceContract(Namespace="http://Lemon")]    
    public interface IWcfService
    {
        [OperationContract(AsyncPattern=true)]                
        IAsyncResult BeginFetch(string fetchType, PackagedDTO fetchCriteria, AsyncCallback callback, object state);
        PackagedDTO EndFetch(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginUpdate(PackagedDTO msg, AsyncCallback callback, object state);
        PackagedDTO EndUpdate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDelete(string deleteType, PackagedDTO deleteCriteria, AsyncCallback callback, object state);
        PackagedDTO EndDelete(IAsyncResult asyncResult);
    }
}