using System;
using System.Runtime.InteropServices;

namespace DroneQualIT.Lecture
{
    internal static class DisableConsoleQuickEdit
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int stdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr consoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr consoleHandle, uint dwMode);

        internal static bool Go()
        {
            var consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            if (!GetConsoleMode(consoleHandle, out uint consoleMode))
                return false;

            consoleMode &= ~ENABLE_QUICK_EDIT;

            if (!SetConsoleMode(consoleHandle, consoleMode))
                return false;

            return true;
        }
    }
}
