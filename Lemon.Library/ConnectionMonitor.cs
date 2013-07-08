using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Lemon.Base
{
    /// <summary>
    /// This class has three main functions related to SQL database connections.  It will only work for SQL Server 2005.
    /// First, it can listen to a change in a GUID field.  If this field changes, an event will be raised.
    /// In inFlow, this is used to listen to changes in a DatabaseSession, so that an administrator can
    /// kick off all other users.
    /// Secondly, it helps clear the connection pool upon disconnection.  This will reduce the number of errors
    /// that will happen after reconnection.
    /// http://www.ondotnet.com/pub/a/dotnet/2004/02/09/connpool.html
    /// Thirdly, it provides an (experimental) event that is triggered when the database is connected or disconnected.
    /// To activate it, the SqlGuidQuery needs to be set to a SQL query that retrieves a GUID.
    /// </summary>
    public class ConnectionMonitor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ConnectionMonitor instance;
        public static ConnectionMonitor Instance { get { if(instance == null) instance = new ConnectionMonitor(); return instance; } }
        
        /// For inFlow, this should be set to "SELECT DatabaseSession FROM dbo.SYS_Metadata"
        /// If this is left as null, then the ConnectionMonitor will remain inactive.
        /// </summary>
        public string SqlGuidQuery { get; set; }

        private Guid _DatabaseSession = Guid.Empty;
        public Guid DatabaseSession { get { return _DatabaseSession; } set { _DatabaseSession = value; } }

        private object readLock = new object();
        private object writeLock = new object();

        private bool _IsListening = false;

        public ConnectionMonitor()
        {
            //I think we always want to have this enabled now for the push notifications
            SqlGuidQuery = "SELECT DatabaseSession FROM dbo.SYS_MetaData";
        }

        public bool IsListening 
        { 
            get 
            { 
                lock (readLock) 
                { 
                    return _IsListening; 
                } 
            } 
            set 
            { 
                if(_IsListening != value)
                {
                    if(value)
                        log.Info("ConnectionMonitor is now actively monitoring the database");
                    else
                        log.Info("ConnectionMonitor has been disconnected from the database");

                    lock (writeLock) 
                    { 
                        lock (readLock) 
                        { 
                            _IsListening = value; 
                        } 
                    }
                    RaiseDatabaseConnectionChanged(_IsListening);
                }
            }
        }


        #region DatabaseSessionChanged Event
        public delegate void DatabaseSessionChangedHandler(Guid newDatabaseSession);
        public event DatabaseSessionChangedHandler DatabaseSessionChanged;

        private void RaiseDatabaseSessionChanged(Guid newDatabaseSession)
        {
            DatabaseSessionChangedHandler h = DatabaseSessionChanged;
            if (h != null)
            {
                //Queue up a request to raise update events asynchronously
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(RaiseDatabaseSessionChangedHelper), newDatabaseSession);
            }
        }

        private void RaiseDatabaseSessionChangedHelper(object state)
        {
            Guid guid = (Guid)state;

            DatabaseSessionChangedHandler h = DatabaseSessionChanged;
            if (h != null)
            {
                h(guid);
            }
        }
        #endregion


        #region DatabaseConnectionChanged Event
        public delegate void DatabaseConnectionChangedHandler(bool isConnected);
        public event DatabaseConnectionChangedHandler DatabaseConnectionChanged;

        private void RaiseDatabaseConnectionChanged(bool isConnected)
        {
            DatabaseConnectionChangedHandler h = DatabaseConnectionChanged;
            if (h != null)
            {
                //Queue up a request to raise update events asynchronously
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(RaiseDatabaseConnectionChangedHelper), isConnected);
            }
        }

        private void RaiseDatabaseConnectionChangedHelper(object state)
        {
            bool isConnected = (bool)state;

            DatabaseConnectionChangedHandler h = DatabaseConnectionChanged;
            if (h != null)
            {
                h(isConnected);
            }
        }
        #endregion


        //This may throw an exception if it's on a bad connection or something.
        public bool StartListening(SqlConnection cn)
        {
            if (SqlGuidQuery == null) return false;

            bool raiseEvent = false;
            lock(writeLock)
            {
                try
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = @"SET ANSI_NULLS ON
                                            SET ANSI_PADDING ON
                                            SET ANSI_WARNINGS ON
                                            SET CONCAT_NULL_YIELDS_NULL ON
                                            SET QUOTED_IDENTIFIER ON
                                            SET NUMERIC_ROUNDABORT OFF
                                            SET ARITHABORT ON
                                            ";
                        cmd.ExecuteNonQuery();
                    }

                    //If we're not already listening, then start up the SqlDependency listener.
                    if (!this.IsListening)
                    {
                        log.Info("Restarting Query Notification connection to database");
                        
                        //Remove any existing dependency connection, then create a new one
                        SqlDependency.Stop(WinterspringConnectionManager.DBConnection);
                        SqlDependency.Start(WinterspringConnectionManager.DBConnection);
                    }

                    using(SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.Notification = null;
                        cmd.CommandText = SqlGuidQuery;
                        cmd.CommandType = CommandType.Text;

                        //Create the dependency object bound to the command.
                        SqlDependency dependency = new SqlDependency(cmd);
                        dependency.OnChange += dependency_OnChange;

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            rdr.Read();
                            Guid newDatabaseSession = rdr.GetGuid(0);
                            //Fire the event for the GUID changing
                            if (DatabaseSession != Guid.Empty && DatabaseSession != newDatabaseSession)
                            {
                                //Mark to raise an event later after the lock is released.
                                raiseEvent = true;
                            }
                            this.DatabaseSession = newDatabaseSession;
                        }
                        //Success!
                        this.IsListening = true;
                    }
                }
                catch
                {
                    this.IsListening = false;
                    throw;
                }
            }
            if(raiseEvent)
                RaiseDatabaseSessionChanged(this.DatabaseSession);

            return this.IsListening;
        }

        void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            log.Debug(String.Format("dependency_OnChange - Info={0};Source={1};Type={2}", e.Info, e.Source, e.Type));

            //Remove the handler since it's only good for a single notification
            SqlDependency dependency = sender as SqlDependency;
            dependency.OnChange -= dependency_OnChange;

            //This happens if the database goes down
            if (e.Info == SqlNotificationInfo.Error || e.Info == SqlNotificationInfo.Restart)
            {
                lock(writeLock)
                {
                    this.IsListening = false;
                    //Since we've been disconnected, clear the connection pool.
                    //Otherwise, when we try to reuse these connections later on,
                    //it'll trigger SqlExceptions later on.
                    log.Info("Clearing Connection Pool after database disconnection or restart");
                    WinterspringConnectionManager.ClearConnectionPool();
                }
                return;
            }

            else if (e.Info == SqlNotificationInfo.Update)
            {
                try
                {
                    //Trigger getting the data and listening again
                    using (WinterspringConnectionManager mgr = WinterspringConnectionManager.GetManager())
                    {
                        SqlConnection cn = mgr.Connection;
                        StartListening(cn);
                    }
                }
                //If there's an error here, just ignore it.  It'll try again to start listening later.
                catch { }
            }
        }
    }

}
