using System;
using System.Text;

namespace ConsoleApp2.HostInfo
{
    class HostInfo
    {
        private static StringBuilder info = new StringBuilder();
        public HostInfo()
        {
            var os = Environment.OSVersion;
            info.Append("Platform: "        + os.Platform                 + "\n");
            info.Append("Version String: "  + os.VersionString            + "\n");
            info.Append("ServicePack: "     + os.ServicePack              + "\n");
            info.Append("User Name: "       + Environment.UserName        + "\n");
            info.Append("SystemDirectory: " + Environment.SystemDirectory + "\n");

            string ip = Utils.GetIP();
            if( ip == "" ) {
                ip = "NO-IP";
            }
            info.Append( "Ip Address: " + ip + "\n" );
        }

        public StringBuilder Info
        {
            get
            {
                return info;
            }
        }
    }
}
