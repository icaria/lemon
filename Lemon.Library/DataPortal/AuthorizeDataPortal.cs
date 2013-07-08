using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Lemon.Base;
using Csla;
using Csla.Security;
using Csla.Server;

namespace Winterspring.DataPortal
{
    class AuthorizeDataPortal : IAuthorizeDataPortal
    {
        public void Authorize(AuthorizeRequest clientRequest)
        {            
            if (ApplicationContext.LocalContext["FirstRun"] == null && ApplicationContext.AuthenticationType == "Custom")
            {
                // the firstrun check is required, because server-side code
                // can call the data portal, so the data portal may be invoked
                // many times to handle a single client request
                // LocalContext is used to ensure this value is per-user, 
                // because the application server is probably servicing
                // many users at once on different threads
                ApplicationContext.LocalContext["FirstRun"] = false;
                ApplicationContext.User = new UnauthenticatedPrincipal();

                //var token = ApplicationContext.ClientContext["AuthenticationToken"] as string;
                //if (String.IsNullOrEmpty(token))
                //{
                //    ApplicationContext.User = new UnauthenticatedPrincipal();
                //}

            }
        }
    }
}
