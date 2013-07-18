using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Lemon.Base
{
    public class BaseConstants
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int _TimedLockWaitSeconds = -1;

        public static int TimedLockWaitSeconds
        {
            get
            {
                if (_TimedLockWaitSeconds == -1)
                {
                    bool parseOk = int.TryParse(ConfigurationManager.AppSettings["TimedLockWaitSeconds"], out _TimedLockWaitSeconds);

                    if (!parseOk || _TimedLockWaitSeconds < 1)
                    {
                        log.Error("TimedLockWaitSeconds in app.config value is incorrect (needs to be an integer).  Setting it to default 600.");
                        _TimedLockWaitSeconds = 600;
                    }
                    log.Info("Constants.TimedLockWaitSeconds : " + _TimedLockWaitSeconds);
                }
                return _TimedLockWaitSeconds;
            }
        }
    }
}
