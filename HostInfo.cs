using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ConsoleApp2.HostInfo
{
    class HostInfo
    {
        private static StringBuilder _info = new StringBuilder();
        private static StringBuilder _HWid;

        public HostInfo()
        {
            var os = Environment.OSVersion;
            _info.Append("Platform: "        + os.Platform                 + "\n");
            _info.Append("Version String: "  + os.VersionString            + "\n");
            _info.Append("ServicePack: "     + os.ServicePack              + "\n");
            _info.Append("User Name: "       + Environment.UserName        + "\n");
            _info.Append("SystemDirectory: " + Environment.SystemDirectory + "\n");

            string ip = Utils.GetIP();
            if( ip == "" ) {
                ip = "NO-IP";
            }
            _info.Append( "Ip Address: " + ip + "\n" );
            _HWid = ComputeHWid();
        }

        private static StringBuilder ComputeHWid()
        {
            return ComputeHash(
                Environment.ProcessorCount + Environment.UserName +
                Environment.MachineName + Environment.OSVersion + new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize);
        }

        private static StringBuilder ComputeHash(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
            StringBuilder hashValue = new StringBuilder();
            foreach (byte b in hashBytes) {
                hashValue.Append(b.ToString("x2"));
            }
            return hashValue;
        }

        public StringBuilder Info
        {
            get
            {
                return _info;
            }
        }

        public StringBuilder Hwid
        {
            get
            {
                return _HWid;
            }
        }
    }
}
