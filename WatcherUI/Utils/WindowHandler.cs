using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WatcherUI.Utils
{
    public static class WindowHandler
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT rectangle);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static bool IsWindowMinimized(IntPtr hWnd)
        {
            return IsIconic(hWnd);
        }

        public static void SetFocus(IntPtr hWnd)
        {
            SetForegroundWindow(hWnd);
        }
        
        public static Rectangle GetWindowPosition(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out RECT pos);

            Rectangle position = new Rectangle
            {
                X = pos.Left + 8,
                Y = pos.Top,
                Width = pos.Right - pos.Left,
                Height = pos.Bottom - pos.Top
            };


            return position;
        }

        public  static IntPtr GetWindowHandleByName(string wName)
        {
            IntPtr hWnd = IntPtr.Zero;

            hWnd = Process.GetProcesses().Where(p => p.MainWindowTitle.Contains(wName)).First().MainWindowHandle;

            return hWnd;
        }
    }
}
