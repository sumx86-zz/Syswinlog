using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using Syswinlog.Classes.StatusLog;

namespace Syswinlog.Classes.Startup
{
    class Startup
    {
        private static readonly string _keyID = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        // directory of log file
        private const string logDir = @"/SysWinlog32/logs";
        // path to the executable
        private static string _exePath = String.Empty;
        // startup folder
        private static string _startup = String.Empty;

        public static void Init()
        {
            _exePath = Application.ExecutablePath;
            _startup = Utils.GetStartupDirectory();

            string syswin = Utils.GetAppDataFolder() + logDir;
            if (!Directory.Exists(syswin)) {
                Directory.CreateDirectory(syswin);
            }
            AddToStartup();
        }

        /// <summary>
        ///     Add the program to the startup folder if it exists, otherwise add it to the registry
        /// </summary>
        /// <returns> Doesn't return a value </returns>
        public static void AddToStartup()
        {
            if ( _startup == "" ) {
                try {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(_keyID, true)) {
                        rk.SetValue("syswinlog32", _exePath);
                    }
                }
                catch (Exception ex)
                {
                    StatusLog.StatusLog.Log("Error adding to registry!");
                }
                return;
            }
            string startupPath = _startup + @"/" + Path.GetFileName(_exePath);
            if (!System.IO.File.Exists(startupPath)) {
                System.IO.File.Copy(_exePath, startupPath);
            }
        }
    }
}
