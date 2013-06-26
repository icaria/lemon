using System;
using System.Data;
using System.Data.SqlClient;

namespace Winterspring.Lemon.Base
{
    [Serializable]
    internal class CmdTestConnection : WinterspringCommandBase<CmdTestConnection>
    {
        public CmdTestConnection()
        {
        }

        public CmdTestConnection(int timeoutSeconds)
        {
            this._TimeoutSeconds = timeoutSeconds;
        }

        private int _TimeoutSeconds = 10;
        public int TimeoutSeconds { get { return _TimeoutSeconds; } set { _TimeoutSeconds = value; } }

        protected override void DataPortal_ExecuteBody()
        {
            string mgrName = WinterspringConnectionManager.DefaultDBConnection;
            int? timeout = null;
            mgrName = "TestConnection";
            timeout = TimeoutSeconds;

            using (WinterspringConnectionManager mgr = WinterspringConnectionManager.GetManager(mgrName, timeout))
            {
                SqlConnection cn = mgr.Connection;
                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = CommandType.Text;
                    cm.CommandText = "SELECT 1";
                    cm.ExecuteNonQuery();
                }
            }
        }
    }
}