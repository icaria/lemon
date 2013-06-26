using System;
using System.Collections.Generic;
using System.Text;

namespace Winterspring.Lemon.Base
{
    public class ApplicationFlowException : System.Exception
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ApplicationFlowException(string msg)
            : base(msg)
        {
            log.Fatal("=========== ApplicationFlowException Start ===============");
            log.Fatal(msg);
            log.Fatal("=========== ApplicationFlowException End ===============");
        }

        public static void ThrowNewApplicationFlowException(string msg)
        {
#if DEBUG
                throw new ApplicationFlowException(msg);
#else
                log.Fatal("=========== ApplicationFlowException Start ===============");
                log.Fatal(msg);
                log.Fatal("=========== ApplicationFlowException End ===============");
#endif
            }
    }
}
