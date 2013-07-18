using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Lemon.Base;


namespace Lemon.Base
{
    public class ConnectionSettings
    {
        public ConnectionSettings()
        {
            ServerName = String.Empty;
            ServerPassword = String.Empty;
            Server = "INFLOWSQL";
            UseSample = false;
            PasswordBinary = null;
            ConnectWithServerNameFirst = true;
        }

        public string ServerName { get; set; }
        public string Server { get; set; }
        public bool UseSample { get; set; }
        public byte[] PasswordBinary { get; set; }
        public string IpAddress { get; set; }
        public bool ConnectWithServerNameFirst { get; set; }

        [XmlIgnore]
        public string ServerPassword { get; set; }

        public static ConnectionSettings LoadFromXML(string filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }
            else
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ConnectionSettings));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    ConnectionSettings settings = deserializer.Deserialize(reader) as ConnectionSettings;
                    settings.ServerPassword = BasicEncryption.DecryptPasswordBinary(settings.PasswordBinary);
                    return settings;
                }
            }
        }

        public void SaveToXML(string filePath)
        {

            if (!String.IsNullOrEmpty(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConnectionSettings));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    PasswordBinary = BasicEncryption.EncryptPasswordBinary(ServerPassword);
                    serializer.Serialize(writer, this);
                }
            }
        }
    }
}
