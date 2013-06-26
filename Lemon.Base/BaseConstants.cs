using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Winterspring.Lemon.Base
{
    public class BaseConstants
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string _DefaultDBConnection;

        internal static string DefaultDBConnection
        {
            get
            {
                if (_DefaultDBConnection == null)
                {
                    //Read the connection string from the database.  This serves as an override.

                    ConnectionStringSettings c = ConfigurationManager.ConnectionStrings["DBConnection"];

                    if (c == null || c.ConnectionString == null || c.ConnectionString == "")
                    {
                        log.Error("DBConnection in app.config is either not there or the value is empty.");
                        _DefaultDBConnection = "Data Source=.\\INFLOWSQL;Initial Catalog=inFlow;Integrated Security=False;User Id=sa;Password=happycustomer;Connection Timeout=15";
                    }
                    else
                    {
                        _DefaultDBConnection = c.ConnectionString;
                    }
                    log.Info("Constants.DBConnection : " + _DefaultDBConnection);
                }
                return _DefaultDBConnection;
            }
        }

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


        private static int _DebugRandomConnectionFailurePercentage = -1;

        public static int DebugRandomConnectionFailurePercentage
        {
            get
            {
                if (_DebugRandomConnectionFailurePercentage == -1)
                {
                    int percent = -1;
                    bool parseOk = int.TryParse(ConfigurationManager.AppSettings["DebugRandomConnectionFailurePercentage"], out percent);

                    if (!parseOk || percent < 0)
                    {
                        log.Error("DebugRandomConnectionFailurePercentage in app.config value is incorrect (needs to be a percentage).  Setting it to default 0.");
                        percent = 0;
                    }
                    _DebugRandomConnectionFailurePercentage = percent;
                    log.Info("Constants.DebugRandomConnectionFailurePercentage: " + percent);
                }
                return _DebugRandomConnectionFailurePercentage;
            }
        }

    }
}
