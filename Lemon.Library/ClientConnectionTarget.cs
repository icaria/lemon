using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;
using Winterspring.Extensions;
using log4net;

namespace Lemon.Base
{
    public class ClientConnectionTarget
    {
        private static readonly ClientConnectionTarget _instance = new ClientConnectionTarget();
        public static ClientConnectionTarget Instance { get { return _instance; } }

        private string _registryRoot = "Software\\inFlow Inventory v3";
        private const string EndpointNameKey = "EndpointName";
        private const string HostNameKey = "HostName";
        public static ILog Log = LogManager.GetLogger(typeof(ClientConnectionTarget));

        public bool ConnectedLocally { get { return IsLocalConnection(HostName); } }

        private string _hostName;
        public string HostName
        {
            get { return _hostName ?? (_hostName = LoadHostName()); }
        }

        private string LoadHostName()
        {
            try
            {
                //Null means we haven't initialized it. String.Empty means we forced a disconnect maybe
                return Registry.CurrentUser.OpenSubKey(_registryRoot).Try(key => key.GetValue(HostNameKey, null) as string) ??
                    Registry.LocalMachine.OpenSubKey(_registryRoot).Try(key => key.GetValue(HostNameKey) as string);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to load host name from registry", ex);
                return Dns.GetHostName();
            }
        }

        public void Initialize(string connectionSettingsPath)
        {
            var settings = ConnectionSettings.LoadFromXML(connectionSettingsPath);
            //Set the registry settings from the connection settings file
            Save(settings.ServerName, settings.UseSample);
        }

        public bool IsConnectedByIpAddress
        {
            get
            {
                IPAddress ipAddress;
                return IPAddress.TryParse(HostName, out ipAddress);
            }
        }

        private string _endpointName;
        public string EndpointName
        {
            get { return _endpointName ?? (_endpointName = LoadEndpointName()); }
            set { _endpointName = value; }
        }

        public bool IsUsingSampleData { get { return String.Equals(EndpointName, "inFlowSample", StringComparison.InvariantCultureIgnoreCase); } }
        public bool IsUnknownConnection { get { return String.IsNullOrEmpty(HostName); } }

        public static string LocalHostName { get { return "localhost"; } }

        private string LoadEndpointName()
        {
            try
            {
                return Registry.CurrentUser.OpenSubKey(_registryRoot).Try(key => key.GetValue(EndpointNameKey) as string) ?? "inFlow";
            }
            catch
            {
                return "inFlow";
            }
        }

        public void Save(string hostName, bool useSample)
        {
            try
            {
                _hostName = hostName;
                EndpointName = useSample ? "inFlowSample" : "inFlow";
                Registry.CurrentUser.CreateSubKey(_registryRoot).TryDo(key => key.SetValue(HostNameKey, hostName));
                Registry.CurrentUser.CreateSubKey(_registryRoot).TryDo(key => key.SetValue(EndpointNameKey, EndpointName));
            }
            catch (Exception ex)
            {
                Log.Info(ex);
            }
        }

        public bool IsLocalConnection(string hostName)
        {
            hostName = hostName.ToLower();
            return hostName == "." || hostName == "localhost" || hostName == "127.0.0.1" || hostName == Dns.GetHostName().ToLower();
        }
    }
}
