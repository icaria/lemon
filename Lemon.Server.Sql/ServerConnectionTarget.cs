using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;
using Lemon.Base;
using System.Net;

namespace Lemon.Base
{
    public class ServerConnectionTarget
    {
        //Change this for now to use the inFlow Inventory v3 path instead of just inFlow Inventory
        public const string RootRegistryPath = "Software\\Lemon";
        public const string HostnameKey = "Hostname";
        public const string PwdHKLMUnencryptedKey = "SQLPwd";
        public const string PwdHKLMEncryptedKey = "SQLPwdBinary";
        public const string UseSampleKey = "UseSample";
        public const string ServerKey = "Server";
        public const string RealDatabaseKey = "Database";
        public const string DatabaseUserKey = "DatabaseUser";
        public const string CustomConnectionStringKey = "CustomConnectionString";
        public const string LocalHostName = ".";
        public const string SampleDatabaseName = "LemonSample";

        public string RegistryPath { get { return String.Format("{0}\\{1}", RootRegistryPath, _databaseProfile); } }
        private readonly string _databaseProfile;
        public string HostName { get; private set; }
        public string Password { get; private set; }
        public bool IsSampleDatabase { get; private set; }
        public string Server { get; private set; }
        public string RealDatabase { get; private set; }
        public string DatabaseUser { get; private set; }
        public string CustomConnectionString { get; private set; }

        public ServerConnectionTarget(string databaseProfile)
        {
            _databaseProfile = databaseProfile;
            IsSampleDatabase = String.Equals(databaseProfile, SampleDatabaseName, StringComparison.InvariantCultureIgnoreCase);
            Initialize();
        }

        public static void Initialize(ServerConnectionTarget instance)
        {
            Instance = instance;
        }

        public static ServerConnectionTarget Instance { get; private set; }

        public void Initialize()
        {
            HostName = LoadHostname();
            Password = LoadPassword();
            Server = LoadServer();
            RealDatabase = LoadRealDatabase();
            DatabaseUser = LoadDatabaseUser();
            CustomConnectionString = LoadCustomConnectionString();

            WinterspringConnectionManager.OverrideConnection = ConnectionString;
        }

        public string CurrentDatabase { get { return IsSampleDatabase ? SampleDatabaseName : RealDatabase; } }
        public string ConnectionString { get { return IsCustomConnectionString ? CustomConnectionString : CalculatedConnectionString; } }

        public bool IsCustomConnectionString
        {
            get
            {
                return !string.IsNullOrEmpty(CustomConnectionString);
            }
        }

        //We should probably build this into the client connection target instead
        //public void Initialize(string connectionSettingsPath)
        //{
        //    ConnectionSettings settings = ConnectionSettings.LoadFromXML(connectionSettingsPath);
        //    if (settings == null)
        //    {
        //        //If we cannot find the connection settings file, load the settings from registry
        //        Initialize();
        //    }
        //    else
        //    {
        //        //Set the registry settings from the connection settings file
        //        SaveToRegistry(settings.ServerName, settings.ServerPassword, settings.UseSample, settings.Server);
        //    }
        //}        

        public RegistryKey RegistryKeyLM
        {
            get
            {

                RegistryKey keyLM = null;

                try
                {
                    keyLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                        .OpenSubKey(RegistryPath, true);
                }
                catch (Exception)
                {
                    try
                    {
                        // open it as read-only
                        keyLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                            .OpenSubKey(RegistryPath, false);
                    }
                    catch (Exception) { }
                }

                if (keyLM == null)
                {
                    try
                    {
                        keyLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                            .CreateSubKey(RegistryPath);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return keyLM;
            }
        }
        
        public void SavePasswordToHKLMEncrypted(string passwordPlainText)
        {
            RegistryKeyLM.SetValue(PwdHKLMEncryptedKey, BasicEncryption.EncryptPasswordBinary(passwordPlainText), RegistryValueKind.Binary);
        }

        public void ChangePassword(string newPasswordPlainText)
        {
            Password = newPasswordPlainText;
            SavePasswordToHKLMEncrypted(newPasswordPlainText);
        }

        private string LoadHostname()
        {
            //If the registry key isn't present, then default it to "." (aka localhost) for standalones and 
            //empty for clients (since clients cant connect locally).  This happens just after
            //a fresh install (i.e. the installer doesn't set the registry key).
            //Another case is that the registry key might be explicitly wiped out and set to blank when the user
            //hits disconnect.  This means that the target Server Name (hostname) is unknown.            
            return RegistryKeyLM.GetValue(HostnameKey, InstallOptionEnum.GetInstallOption() == InstallOption.ClientOnly ? String.Empty : ".").ToString();
        }

        public bool IsUnknownConnection()
        {
            //A blank hostname means that we don't know where to connect.  See LoadHostname()
            return string.IsNullOrEmpty(HostName);
        }

        public string LoadPassword()
        {
            return LoadPasswordFromHKLMEncrypted() ?? LoadPasswordFromHKLMUnencrypted();
        }

        public string LoadPasswordFromHKLMEncrypted()
        {
            RegistryKey keyLM = RegistryKeyLM;

            var b = (byte[])keyLM.GetValue(PwdHKLMEncryptedKey, new byte[0]);
            if (b != null)
            {
                string pwd = BasicEncryption.DecryptPasswordBinary(b);
                if (!string.IsNullOrEmpty(pwd))
                {
                    return pwd;
                }
            }
            return null;
        }

        public string LoadPasswordFromHKLMUnencrypted()
        {
            RegistryKey keyLM = RegistryKeyLM;
            return ((keyLM == null) ? "" : keyLM.GetValue(PwdHKLMUnencryptedKey, "lemonade").ToString());
        }

        private string LoadServer()
        {
            return RegistryKeyLM.GetValue(ServerKey, "LEMONSQL").ToString();
        }

        private string LoadRealDatabase()
        {
            return RegistryKeyLM.GetValue(RealDatabaseKey, "Lemon").ToString();
        }

        private string LoadDatabaseUser()
        {
            return RegistryKeyLM.GetValue(DatabaseUserKey, "sa").ToString();
        }

        private string LoadCustomConnectionString()
        {
            return RegistryKeyLM.GetValue(CustomConnectionStringKey, "").ToString();
        }

        private string CalculatedConnectionString
        {
            get
            {
                return String.Format(@"Data Source={0}{1};Initial Catalog={2};Integrated Security=False;User Id={3};Password={4};Connection Timeout=15",
                    HostName,
                    Server == "" ? "" : @"\" + Server,
                    CurrentDatabase, DatabaseUser, Password);
            }
        }

        public void OverrideConnection(string hostName, string server, string databaseUser, string password)
        {
            HostName = hostName;
            Server = server;
            DatabaseUser = databaseUser;
            Password = password;

            WinterspringConnectionManager.OverrideConnection = ConnectionString;
        }
    }
}
