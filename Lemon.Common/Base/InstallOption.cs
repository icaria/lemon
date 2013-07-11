using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;

namespace Lemon.Base
{
    public enum InstallOption
    {
        StandaloneOrServer = 1,
        ClientOnly = 2
    }

    public class InstallOptionEnum
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string REGISTRY_KEY_NAME = @"SOFTWARE\Lemon";
        private const string REGISTRY_INSTALL_OPTION = "InstallOption";

        public static InstallOption? GetInstallOption()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY_NAME, false);

                object o = key.GetValue(REGISTRY_INSTALL_OPTION);
                if (o == null) return null;
                if (System.Enum.IsDefined(typeof(InstallOption), o))
                    return (InstallOption)o;
                int val = 0;
                if (Int32.TryParse(o.ToString(), out val))
                {
                    if (System.Enum.IsDefined(typeof(InstallOption), val))
                        return (InstallOption)val;
                    else
                        return null;
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }
    }
}
