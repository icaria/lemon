using System;
using System.Threading.Tasks;
using Csla.Server;
using Csla.Security;
using Winterspring.Extensions;
using Lemon.Common;
using ProtoBuf.Meta;

namespace Winterspring.DataPortal
{       
    public class WcfService : IWcfService
    {
        private DataPortalContext GetDataPortalContext(string authenticationToken)
        {
            return new DataPortalContext(new UnauthenticatedPrincipal(), true, "en-US", "en-US", new Csla.Core.ContextDictionary { { "AuthenticationToken", authenticationToken } }, new Csla.Core.ContextDictionary());
        }

        private async Task<PackagedDataTransferObject> FetchAsync(string fetchType, PackagedDataTransferObject fetchCriteria)
        {
            var unpackagedMessage = fetchCriteria.UnpackageDataTransferObject();
            var portal = new Csla.Server.DataPortal();
            try
            {                
                var type = Type.GetType(fetchType);                                 
                var result = await portal.Fetch(type, unpackagedMessage.Body, GetDataPortalContext(unpackagedMessage.Header.AuthenticationToken), false);

                return type.GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0 ?
                    PackagedDataTransferObject.PackageDTO(result.ReturnObject) : PackagedDataTransferObject.PackageBO(result.ReturnObject);
            }
            catch (Exception ex)
            {
                return PackagedDataTransferObject.PackageServerException(ex);                
            }
        }

        public async Task<PackagedDataTransferObject> UpdateAsync(PackagedDataTransferObject msg)
        {
            var portal = new Csla.Server.DataPortal();
            var unpackagedDto = msg.UnpackageDataTransferObject();
            try
            {
                var result = await portal.Update(unpackagedDto.UnpackageBO(), GetDataPortalContext(unpackagedDto.Header.AuthenticationToken), false);

                var type = Type.GetType(unpackagedDto.Header.Type);
                return type.GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0 ?
                    PackagedDataTransferObject.PackageDTO(result.ReturnObject) : PackagedDataTransferObject.PackageBO(result.ReturnObject);
            }
            catch (Exception ex)
            {
                return PackagedDataTransferObject.PackageServerException(ex);
            }
        }

        public async Task<PackagedDataTransferObject> DeleteAsync(string deleteType, PackagedDataTransferObject deleteCriteria)
        {
            var unpackagedDto = deleteCriteria.UnpackageDataTransferObject();
            var portal = new Csla.Server.DataPortal();
            try
            {
                var type = Type.GetType(deleteType);
                var result = await portal.Delete(type, unpackagedDto.Body, GetDataPortalContext(unpackagedDto.Header.AuthenticationToken), false);
                return type.GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0 ?
                    PackagedDataTransferObject.PackageDTO(result.ReturnObject) : PackagedDataTransferObject.PackageBO(result.ReturnObject);
            }
            catch (Exception ex)
            {
                return PackagedDataTransferObject.PackageServerException(ex);
            }
        }

        public IAsyncResult BeginFetch(string fetchType, PackagedDataTransferObject fetchCriteria, AsyncCallback callback, object state)
        {            
            return FetchAsync(fetchType, fetchCriteria).ToBegin(callback, state);                        
        }

        public PackagedDataTransferObject EndFetch(IAsyncResult asyncResult)
        {
            return asyncResult.ToEnd<PackagedDataTransferObject>();
        }

        public IAsyncResult BeginUpdate(PackagedDataTransferObject msg, AsyncCallback callback, object state)
        {
            return UpdateAsync(msg).ToBegin(callback, state);
        }

        public PackagedDataTransferObject EndUpdate(IAsyncResult asyncResult)
        {
            return asyncResult.ToEnd<PackagedDataTransferObject>();
        }

        public IAsyncResult BeginDelete(string deleteType, PackagedDataTransferObject deleteCriteria, AsyncCallback callback, object state)
        {
            return DeleteAsync(deleteType, deleteCriteria).ToBegin(callback, state);
        }

        public PackagedDataTransferObject EndDelete(IAsyncResult asyncResult)
        {
            return asyncResult.ToEnd<PackagedDataTransferObject>();
        }
    }
}