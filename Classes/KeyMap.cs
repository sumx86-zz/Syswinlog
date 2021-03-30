using System;
using System.Windows.Forms;
using System.Collections.Generic;

class KeyMap
{
    // use a keymap dict to map keys that require the shift key
    public static Dictionary<char, char> keymap = new Dictionary<char, char>
    {
        {'`','~'},
        {'[','{'},
        {']','}'},
        {',','<'},
        {'.','>'},
        {'/','?'},
        {';',':'},
        {'\'','\"'},
        {'\\','|'},
        {'1','!'},
        {'2','@'},
        {'3','#'},
        {'4','$'},
        {'5','%'},
        {'6','^'},
        {'7','&'},
        {'8','*'},
        {'9','('},
        {'0',')'},
        {'-','_'},
        {'=','+'}
    };

    public static List<int> nonPrintableKeys = new List<int> {
        38, 39, 37, 40, 91, 92, 8, 9, 35,
        20, 160, 161, 162, 163, 17, 16, 27
    };
}

