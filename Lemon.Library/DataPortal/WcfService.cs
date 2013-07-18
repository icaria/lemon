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

        private async Task<PackagedDTO> FetchAsync(string fetchType, PackagedDTO fetchCriteria)
        {
            var unpackagedMessage = fetchCriteria.UnpackageDTO();
            var portal = new Csla.Server.DataPortal();
            try
            {                
                var type = Type.GetType(fetchType);                                 
                var result = await portal.Fetch(type, unpackagedMessage.Body, GetDataPortalContext(unpackagedMessage.Header.AuthenticationToken), false);

                return type.GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0 ?
                    PackagedDTO.PackageDTO(result.ReturnObject) : PackagedDTO.PackageBO(result.ReturnObject);
            }
            catch (Exception ex)
            {
                return PackagedDTO.PackageServerException(ex);                
            }
        }

        public async Task<PackagedDTO> UpdateAsync(PackagedDTO msg)
        {
            var portal = new Csla.Server.DataPortal();
            var unpackagedDto = msg.UnpackageDTO();
            try
            {
                var result = await portal.Update(unpackagedDto.UnpackageBO(), GetDataPortalContext(unpackagedDto.Header.AuthenticationToken), false);

                var type = Type.GetType(unpackagedDto.Header.Type);
                return type.GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0 ?
                    PackagedDTO.PackageDTO(result.ReturnObject) : PackagedDTO.PackageBO(result.ReturnObject);
            }
            catch (Exception ex)
            {
                return PackagedDTO.PackageServerException(ex);
            }
        }

        public async Task<PackagedDTO> DeleteAsync(string deleteType, PackagedDTO deleteCriteria)
        {
            var unpackagedDto = deleteCriteria.UnpackageDTO();
            var portal = new Csla.Server.DataPortal();
            try
            {
                var type = Type.GetType(deleteType);
                var result = await portal.Delete(type, unpackagedDto.Body, GetDataPortalContext(unpackagedDto.Header.AuthenticationToken), false);
                return type.GetCustomAttributes(typeof(DataTransferObjectAttribute), false).Length > 0 ?
                    PackagedDTO.PackageDTO(result.ReturnObject) : PackagedDTO.PackageBO(result.ReturnObject);
            }
            catch (Exception ex)
            {
                return PackagedDTO.PackageServerException(ex);
            }
        }

        public IAsyncResult BeginFetch(string fetchType, PackagedDTO fetchCriteria, AsyncCallback callback, object state)
        {            
            return FetchAsync(fetchType, fetchCriteria).ToBegin(callback, state);                        
        }

        public PackagedDTO EndFetch(IAsyncResult asyncResult)
        {
            return asyncResult.ToEnd<PackagedDTO>();
        }

        public IAsyncResult BeginUpdate(PackagedDTO msg, AsyncCallback callback, object state)
        {
            return UpdateAsync(msg).ToBegin(callback, state);
        }

        public PackagedDTO EndUpdate(IAsyncResult asyncResult)
        {
            return asyncResult.ToEnd<PackagedDTO>();
        }

        public IAsyncResult BeginDelete(string deleteType, PackagedDTO deleteCriteria, AsyncCallback callback, object state)
        {
            return DeleteAsync(deleteType, deleteCriteria).ToBegin(callback, state);
        }

        public PackagedDTO EndDelete(IAsyncResult asyncResult)
        {
            return asyncResult.ToEnd<PackagedDTO>();
        }
    }
}