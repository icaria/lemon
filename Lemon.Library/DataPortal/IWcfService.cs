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
        IAsyncResult BeginFetch(string fetchType, PackagedDataTransferObject fetchCriteria, AsyncCallback callback, object state);
        PackagedDataTransferObject EndFetch(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginUpdate(PackagedDataTransferObject msg, AsyncCallback callback, object state);
        PackagedDataTransferObject EndUpdate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDelete(string deleteType, PackagedDataTransferObject deleteCriteria, AsyncCallback callback, object state);
        PackagedDataTransferObject EndDelete(IAsyncResult asyncResult);
    }
}