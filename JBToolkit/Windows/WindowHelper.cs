using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JBToolkit.Windows
{
    /// <summary>
    /// As in the actual program Window rather then the 'Windows' operating system... Used to focus windows (bring to front etc)
    /// </summary>
    public class WindowHelper
    {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(int hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop,
            EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        // Define the callback delegate's type.
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        public static RECT GetWindowRect(IntPtr hWnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);

            return rect;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        // Save window titles and handles in these lists.
        private static List<IntPtr> WindowHandles;
        private static List<string> WindowTitles;

        // Return a list of the desktop windows' handles and titles.
        public static void GetDesktopWindowHandlesAndTitles(
            out List<IntPtr> handles, out List<string> titles)
        {
            WindowHandles = new List<IntPtr>();
            WindowTitles = new List<string>();

            if (!EnumDesktopWindows(IntPtr.Zero, FilterCallback,
                IntPtr.Zero))
            {
                handles = null;
                titles = null;
            }
            else
            {
                handles = WindowHandles;
                titles = WindowTitles;
            }
        }

        // We use this function to filter windows.
        // This version selects visible windows that have titles.
        private static bool FilterCallback(IntPtr hWnd, int lParam)
        {
            // Get the window's title.
            StringBuilder sb_title = new StringBuilder(1024);
            int _ = GetWindowText(hWnd, sb_title, sb_title.Capacity);
            string title = sb_title.ToString();

            // If the window is visible and has a title, save it.
            if (IsWindowVisible(hWnd) &&
                string.IsNullOrEmpty(title) == false)
            {
                WindowHandles.Add(hWnd);
                WindowTitles.Add(title);
            }

            // Return true to indicate that we
            // should continue enumerating windows.
            return true;
        }

        /// <summary>
        /// Move and / or resize window
        /// </summary>
        public static void MoveWindow(Process process, int x, int y, int width, int hight)
        {
            MoveWindow(process.MainWindowHandle, x, y, width, hight, true);
        }

        /// <summary>
        /// Move and / or resize window
        /// </summary>
        public static void MoveWindow(IntPtr handle, int x, int y, int width, int hight)
        {
            MoveWindow(handle, x, y, width, hight, true);
        }

        /// <summary>
        /// Focus's the main window of the process to the very front
        /// </summary>
        /// <param name="processName"></param>
        public static void BringMainWindowToFront(IntPtr hWnd)
        {
            // the window is hidden so try to restore it before setting focus.
            ShowWindow(hWnd, ShowWindowEnum.Restore);

            // set user the focus to the window
            SetForegroundWindow(hWnd);
        }

        /// <summary>
        /// Focus's the main window of the process to the very front
        /// </summary>
        /// <param name="processName"></param>
        public static void BringMainWindowToFront(string processName)
        {
            // get the process
            Process bProcess = Process.GetProcessesByName(processName).FirstOrDefault();

            // check if the process is nothing or not.
            if (bProcess != null)
            {
                // get the hWnd of the process
                IntPtr hwnd = bProcess.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(bProcess.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(bProcess.MainWindowHandle);
            }
            else
            {
                // the process is nothing, so start it
                Process.Start(processName);
            }
        }
    }
}
