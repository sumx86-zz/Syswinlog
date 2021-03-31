using System;
using System.Text;
using System.IO;

namespace Syswinlog.Classes.StatusLog
{
    class StatusLog
    {
        private const string _logFile = @"/SysWinlog32/logs/status.txt";

        public static void Log(string message)
        {
            string date = DateTime.Now.ToString("dd/MMMM/yyyy - HH:mm:ss tt");
            using (StreamWriter sw = new StreamWriter(Utils.GetAppDataFolder() + _logFile, true, Encoding.Unicode)) {
                sw.Write(date + " -- " + message + "\n");
            }
        }
    }
}
