using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Csla;
using Csla.Data;

namespace Lemon.Base
{
    /// <summary>
    /// This is a class to manage a connection so that the same connection will be used by
    /// different operations within the DataPortal without necessarily having to pass
    /// the SqlConnection around.
    /// Using a single connection is necessary for TransactionScope transactions without
    /// using slower distributed transactions (MSDTC).
    /// It is saved in CSLA's LocalContext, which doesn't get passed through the DataPortal,
    /// so it'll stay on the server side.
    /// This uses a reference counting mechanism to dispose the Connection as soon as possible.
    /// http://forums.lhotka.net/forums/thread/15910.aspx
    /// http://forums.lhotka.net/forums/thread/9781.aspx
    /// http://forums.lhotka.net/forums/thread/3797.aspx
    /// http://www.lhotka.net/Article.aspx?area=4&id=39c79955-ddc9-42c7-a657-d5c2ed49975e
    /// </summary>
    public class WinterspringConnectionManager : IDisposable
    {
        public const string DefaultDBConnection = "DefaultDBConnection";

        private SqlConnection connection;
        private string name;
        private int refCount;
        private static MappingSource _mappingSource = null;

        //This functionality may no longer be necessary, so don't set any retries.
        public const int MaxTries = 1;

        private static string _OverrideConnection = null;
        public static string OverrideConnection { get { return _OverrideConnection; } set { _OverrideConnection = value; } }

        public static string DBConnection
        {
            get { return OverrideConnection; // ?? BaseConstants.DefaultDBConnection;
            }
        }

        /// <summary>
        /// Hold the lock this object to prevent any further 
        /// database connections from being made from this
        /// computer in background threads, etc.
        /// e.g. this is used when restoring the database so that nothing else messes things up in the meantime.
        /// </summary>
        public static object ConnectionLock = new object();

        private int? _ConnectionTimeout;
        public int? ConnectionTimeout { get { return _ConnectionTimeout; } set { _ConnectionTimeout = value; } }


        private WinterspringConnectionManager(string name, int? timeout)
        {
            this.name = name;
            this._ConnectionTimeout = timeout;
            Csla.ApplicationContext.LocalContext[name] = this;
        }

        ~WinterspringConnectionManager()
        {
            Dispose(false);
        }

        public static void ClearConnectionPool()
        {
            SqlConnection.ClearAllPools();
        }

        public SqlConnection Connection
        {
            get
            {
                //Respect the connection lock.
                lock (WinterspringConnectionManager.ConnectionLock)
                { }

                lock (this)
                {
                    int numTries = 0;
                    while (numTries < MaxTries)
                    {
                        ++numTries;
                        try
                        {
                            if (connection == null || connection.State == ConnectionState.Broken || connection.State == ConnectionState.Closed)
                            {
                                string cnStr = WinterspringConnectionManager.DBConnection;
                                //Replace the timeout if necessary
                                if (this._ConnectionTimeout != null)
                                {
                                    cnStr = Regex.Replace(cnStr, @"Connection Timeout\s*=\s*[0-9]*", String.Format("Connection Timeout={0}", (int)this._ConnectionTimeout));
                                }                                
                                this.connection = new SqlConnection(cnStr);
                                connection.Open();
                            }

                            if (!ConnectionMonitor.Instance.IsListening)
                            {
                                ConnectionMonitor.Instance.StartListening(connection);
                            }                            
                            return connection;
                        }
                        catch
                        {
                            //The connection must've been bad.  Discard it and try to make a new one.
                            this.connection = null;

                            //If we've given up, just pass the exception on up.
                            if (numTries >= MaxTries)
                            {
                                throw;
                            }
                        }
                    }

                    return connection;
                }
            }
        }

        public string Name
        {
            get { return name; }
        }

        private void AddRef()
        {
            this.refCount++;
        }

        private void DeRef()
        {
            this.refCount--;
            if (refCount == 0)
            {
                if (connection != null)
                    connection.Dispose();
                Csla.ApplicationContext.LocalContext.Remove(name);
                GC.SuppressFinalize(this);
            }
        }

        public static WinterspringConnectionManager GetManager()
        {
            return GetManager(DefaultDBConnection, null);
        }

#if DEBUG
        private static System.Random random = new Random();
#endif

        public static WinterspringConnectionManager GetManager(string name, int? timeout)
        {
//#if DEBUG
//            if (random.Next(1, 101) <= BaseConstants.DebugRandomConnectionFailurePercentage)
//            {
//                throw new ServerReturnedException(new ServerReturnedError(ServerReturnedError.ErrorNumber.DbConnectionLost, "Debug mode random simulated disconnection"));
//            }
//#endif

            WinterspringConnectionManager mgr;
            if (Csla.ApplicationContext.LocalContext.Contains(name))
            {
                mgr = Csla.ApplicationContext.LocalContext[name] as WinterspringConnectionManager;
            }
            else
            {
                mgr = new WinterspringConnectionManager(name, timeout);
            }
            mgr.AddRef();
            return mgr;
        }

        ///// <exception cref="SqlException">This method uses the database and may throw an exception</exception>
        ///// Throws an exception if it can't connect to the database, otherwise does nothing.
        //public static void TestConnection()
        //{
        //    CmdTestConnection cmd = new CmdTestConnection();
        //    cmd = DataPortal.Execute<CmdTestConnection>(cmd);
        //}

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                DeRef();
        }

        //Crazy bug with LINQ to SQL and compiled queries unless you use the same mapping source when you compile the query and when you run it.
        //http://social.msdn.microsoft.com/forums/en-US/linqprojectgeneral/thread/5fbea9d3-5d73-48ae-9109-bdda4bfec7a3/
        //http://blogs.msdn.com/b/gisenberg/archive/2008/05/20/linq-to-sql-optimizing-datacontext-construction-with-the-factory-pattern.aspx
        public DataContext CreateDataContext() { return CreateDataContext(false); }
        public DataContext CreateDataContext(bool writeable)
        {
            if (_mappingSource == null)
            {
                var dc = new DataContext(Connection) { ObjectTrackingEnabled = writeable };
                _mappingSource = dc.Mapping.MappingSource;
                return dc;
            }
            return new DataContext(Connection, _mappingSource) { ObjectTrackingEnabled = writeable };
        }

        public DataContext CreateWriteableDataContext()
        {
            return CreateDataContext(true);
        }

    }
}
