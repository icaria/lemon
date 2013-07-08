using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Winterspring.Extensions;
using Lemon.Base;
using Lemon.Common;
using log4net;

namespace Lemon.Server.Sql
{
    public static class SqlDataMonitor
    {
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static readonly string ByteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
        public static Action<DataChangedEventArgs> DataChanged;

        public static void Start(ILog log = null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            Listen();
                        }
                        catch (Exception ex)
                        {
                            log.TryDo(l => l.Info(ex.Message, ex));
                        }
                    }
                });
        }

        public static void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private static void OnDataChanged(string message)
        {
            //Sql server writes the byte order mark which messes up the XML Parser... We should strip it out if we detect it
            if (message.StartsWith(ByteOrderMarkUtf8))
            {
                message = message.Remove(0, ByteOrderMarkUtf8.Length);
            }

            var doc = XDocument.Parse(message);
            var e = doc.Root.Try(root => new DataChangedEventArgs
                (
                    (DataChangeOperation)Enum.Parse(typeof(DataChangeOperation), root.Element("op").Try(x => x.Value)),
                    root.Element("table").Try(x => x.Value),
                    root.Descendants("id").Try(ids => ids.Select(x => Convert.ToInt32(x.Value))).ToList()
                ));

            DataChanged.TryDo(h => h(e));
        }

        private static void Listen()
        {
            using (var connection = WinterspringConnectionManager.GetManager("SqlDataMonitor", null).Connection)
            {
                using (var command = connection.CreateCommand())
                {
                    //This stored procedure blocks until it receives a message, so make sure to call this async or on a diff thread
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Cache_WaitForPushNotification";
                    command.CommandTimeout = 0; //Never time out

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = reader.GetString(0);
                            OnDataChanged(message);
                        }
                    }
                }
            }
        }
    }
}
