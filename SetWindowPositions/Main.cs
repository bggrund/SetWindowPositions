using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;

namespace SetWindowPositions
{
    class MainClass
    {
        #region Win32 API

        // Define the FindWindow API function.
        [DllImport("user32.dll", EntryPoint = "FindWindow",
            SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly,
            string lpWindowName);

        // Define the SetWindowPos API function.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd,
            IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
            SetWindowPosFlags uFlags);


        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", CallingConvention = CallingConvention.StdCall)]
        static extern int SetWindowLongA(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

        // Define the SetWindowPosFlags enumeration.
        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            SynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool DestroyWindow(IntPtr hWnd);

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool CloseWindow(IntPtr hWnd);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        static void CloseWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowLongA(IntPtr hWnd, int index);

        #endregion
        static string windowDataFile = "window-data.json";

        public static void Main(string[] args)
        {
            #region MyRegion
            /*
                //Thread.Sleep(5000);

                // Sticky Notes =========================================================
                IntPtr stickyNotesHwnd = FindWindowByCaption(IntPtr.Zero, "Sticky Notes");

                //while (stickyNotesHwnd == IntPtr.Zero)
                //{
                //    Thread.Sleep(1000);
                //    stickyNotesHwnd = FindWindowByCaption(IntPtr.Zero, "Sticky Notes");
                //}

                //RECT rect1 = new RECT();
                //GetWindowRect(stickyNotesHwnd, ref rect1);

                //SetWindowPos(stickyNotesHwnd, IntPtr.Zero, 2553, 358, 216, 1047, 0);


                // Chrome ===============================================================
                Process[] chromeProcesses = Process.GetProcessesByName("chrome");

                foreach (Process p in chromeProcesses)
                {
                    if (!string.IsNullOrWhiteSpace(p.MainWindowTitle))
                    {
                        IntPtr hwnd = p.MainWindowHandle;

                        //RECT rect = new RECT();
                        //GetWindowRect(hwnd, ref rect);

                        //ShowWindow(hwnd, 1);

                        //SetWindowPos(hwnd, IntPtr.Zero, 2755, 358, 1732, 1047, 0);
                    }
                }


                // Brown Noise ==============================================
                Process[] brownNoiseProcesses = Process.GetProcessesByName("chrome");

                List<string> chromeTitles = new List<string>(WindowsByClassFinder.WindowTitlesForClass("Chrome_WidgetWin_1"));
                List<IntPtr> chromeWindows = new List<IntPtr>(WindowsByClassFinder.WindowsMatching("Chrome_WidgetWin_1"));

                foreach (string s in chromeTitles)
                {
                    if (s.Contains("White Noise"))
                    {
                        int idx = chromeTitles.IndexOf(s);
                        IntPtr hwnd = chromeWindows[idx];

                        while (idx < chromeTitles.Count - 1)
                        {
                            idx = chromeTitles.IndexOf(s, idx + 1);
                            if (idx >= 0)
                            {
                                CloseWindow(chromeWindows[idx]);
                            }
                            else
                            {
                                break;
                            }
                        }

                        RECT rect = new RECT();
                        GetWindowRect(hwnd, ref rect);

                        ShowWindow(hwnd, 1);

                        // Transparent windows with click through
                        //SetWindowLongA(hwnd, -20, 524288);//GWL_EXSTYLE=-20; WS_EX_LAYERED=524288=&h80000, WS_EX_TRANSPARENT=32=0x00000020L
                        //SetLayeredWindowAttributes(hwnd, 0, 127, 2);// Transparency=51=20%, LWA_ALPHA=2

                        //SetWindowPos(hwnd, new IntPtr(-1), 3082, -454, 397, 327, SetWindowPosFlags.FrameChanged);

                        SetWindowPos(hwnd, new IntPtr(-1), 2742, 136, 436, 73, 0);
                    }
                }


                // Gmail ==============================================

                Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>(OpenWindowGetter.GetOpenWindows());

                bool bFound = false;
                foreach (KeyValuePair<IntPtr, string> p in windows)
                {
                    if (p.Value.Contains("bggrund@gmail"))
                    {
                        if (!bFound)
                        {
                            bFound = true;

                            IntPtr hwnd = p.Key;

                            RECT rect = new RECT();
                            GetWindowRect(hwnd, ref rect);

                            ShowWindow(hwnd, 1);

                            SetWindowPos(hwnd, new IntPtr(-1), 2742, -103, 436, 246, 0);
                        }
                        else // If already found, delete duplicate windows
                        {
                            CloseWindow(p.Key);
                        }
                    }
                }

                // Weather ==============================================

                bFound = false;
                foreach (KeyValuePair<IntPtr, string> p in windows)
                {
                    if (p.Value.Contains("10-Day Weather"))
                    {
                        if (!bFound)
                        {
                            bFound = true;
                            IntPtr hwnd = p.Key;

                            RECT rect = new RECT();
                            GetWindowRect(hwnd, ref rect);

                            ShowWindow(hwnd, 1);

                            SetWindowLongA(hwnd, -20, 0x8);
                            SetWindowPos(hwnd, new IntPtr(-1), 3164, -53, 560, 262, SetWindowPosFlags.FrameChanged);
                        }
                        else
                        {
                            CloseWindow(p.Key);
                        }
                    }
                }

                bFound = false;
                foreach (KeyValuePair<IntPtr, string> p in windows)
                {
                    if (p.Value.Contains("Hourly Weather"))
                    {
                        if (!bFound)
                        {
                            bFound = true;
                            IntPtr hwnd = p.Key;

                            RECT rect = new RECT();
                            GetWindowRect(hwnd, ref rect);

                            ShowWindow(hwnd, 1);

                            SetWindowLongA(hwnd, -20, 0x8);
                            SetWindowPos(hwnd, new IntPtr(-1), 3164, -454, 560, 408, SetWindowPosFlags.FrameChanged);
                        }
                        else
                        {
                            CloseWindow(p.Key);
                        }
                    }
                }


                // MSI Afterburner ==============================================
                //Process[] afterburnerProcesses = Process.GetProcessesByName("MSIAfterburner");

                //Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>(OpenWindowGetter.GetOpenWindows());

                foreach (KeyValuePair<IntPtr, string> p in windows)
                {
                    if (p.Value.Contains("MSI Afterburner"))
                    {
                        IntPtr hwnd = p.Key;

                        RECT rect = new RECT();
                        GetWindowRect(hwnd, ref rect);

                        ShowWindow(hwnd, 1);

                        // Transparent windows with click through
                        SetWindowLongA(hwnd, -20, 524288);//GWL_EXSTYLE=-20; WS_EX_LAYERED=524288=&h80000, WS_EX_TRANSPARENT=32=0x00000020L
                        SetLayeredWindowAttributes(hwnd, 0, 127, 2);// Transparency=51=20%, LWA_ALPHA=2

                        //SetWindowPos(hwnd, IntPtr.Zero, 3689, -454, 225, 664, 0);
                        SetWindowPos(hwnd, new IntPtr(-1), 3689, -454, 225, 664, SetWindowPosFlags.FrameChanged);

                        //SetWindowPos(hwnd, new IntPtr(-1), 3082, -454, 397, 327, 0);

                        break;
                    }
                }

                // Spotify ==============================================================
                Process[] spotifyProcesses = Process.GetProcessesByName("spotify");

                foreach (Process p in spotifyProcesses)
                {
                    if (!string.IsNullOrWhiteSpace(p.MainWindowTitle))
                    {
                        IntPtr hwnd = p.MainWindowHandle;

                        RECT rect = new RECT();
                        GetWindowRect(hwnd, ref rect);

                        ShowWindow(hwnd, 1);

                        SetWindowPos(hwnd, IntPtr.Zero, 2560, -454, 1360, 728, 0);
                    }
                }


                // iTunes ==============================================================
                Process[] itunesProcesses = Process.GetProcessesByName("itunes");

                foreach (Process p in itunesProcesses)
                {
                    if (!string.IsNullOrWhiteSpace(p.MainWindowTitle))
                    {
                        IntPtr hwnd = p.MainWindowHandle;

                        //RECT rect = new RECT();
                        //GetWindowRect(hwnd, ref rect);

                        ShowWindow(hwnd, 1);

                        SetWindowPos(hwnd, IntPtr.Zero, 2762, 358, 1718, 1040, 0);
                    }
                }


                // Discord ==============================================================
                Process[] discordProcesses = Process.GetProcessesByName("Discord");

                foreach (Process p in discordProcesses)
                {
                    if (!string.IsNullOrWhiteSpace(p.MainWindowTitle))
                    {
                        IntPtr hwnd = p.MainWindowHandle;

                        //RECT rect = new RECT();
                        //GetWindowRect(hwnd, ref rect);

                        ShowWindow(hwnd, 1);

                        SetWindowPos(hwnd, IntPtr.Zero, 2762, 358, 1718, 1040, 0);
                    }
                }
                */
            #endregion
            
            if(!File.Exists(windowDataFile))
            {
                File.WriteAllText(windowDataFile, "[]");
            }

            string json = File.ReadAllText(windowDataFile);

            // Read window data
            List<Window> windowData = JsonSerializer.Deserialize<List<Window>>(json, new JsonSerializerOptions { IncludeFields = true });

            // Get all open window names+hwnds
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>(OpenWindowGetter.GetOpenWindows());

            if (args.Length > 0)
            {
                // If no data, prompt user for window names
                if(windowData.Count == 0)
                {
                    Console.WriteLine("Enter names of windows to save. Press enter after each name, then again when complete.");

                    while(true)
                    {
                        string s = Console.ReadLine();
                        if(string.IsNullOrWhiteSpace(s))
                        {
                            break;
                        }
                        else
                        {
                            windowData.Add(new Window() { name = s });
                        }
                    }
                }

                // Save window data
                foreach (Window w in windowData)
                {
                    foreach(KeyValuePair<IntPtr, string> window in windows)
                    {
                        // Found window matching specified name
                        if(window.Value.Contains(w.name))
                        {
                            // Get rect
                            RECT rect = new RECT();
                            GetWindowRect(window.Key, ref rect);
                            w.rect = rect;

                            // Get top-most
                            bool isTopmost = false;
                            int style = GetWindowLongA(window.Key, -20);
                            if ((style & 8) != 0)
                            {   // This is a top-most window
                                isTopmost = true;
                            }
                            w.isTopmost = isTopmost;

                            break;
                        }
                    }
                }
                json = JsonSerializer.Serialize(windowData, new JsonSerializerOptions() { IncludeFields = true, WriteIndented = true });
                File.WriteAllText(windowDataFile, json);
            }
            else // Set window positions
            {
                foreach(Window w in windowData)
                {
                    bool bFound = false;
                    foreach(KeyValuePair<IntPtr, string> window in windows)
                    {
                        // Found window matching specified name
                        if (window.Value.Contains(w.name))
                        {
                            IntPtr hwnd = window.Key;

                            if (!bFound)
                            {
                                bFound = true;

                                // Show window
                                ShowWindow(hwnd, 1);

                                // Set window position
                                SetWindowPos(hwnd, new IntPtr(w.isTopmost ? -1 : 0), w.rect.Left, w.rect.Top, w.rect.Right - w.rect.Left + 1, w.rect.Bottom - w.rect.Top + 1, 0);
                            }
                            else // If already found, delete duplicate windows
                            {
                                CloseWindow(hwnd);
                            }
                        }
                    }
                }
            }
        }
    }

    class Window 
    {
        public string name;
        public RECT rect;
        public bool isTopmost;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    class WindowsByClassFinder
    {
        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr windowHandle, StringBuilder stringBuilder, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);


        /// <summary>Find the windows matching the specified class name.</summary>

        public static IEnumerable<IntPtr> WindowsMatching(string className)
        {
            return new WindowsByClassFinder(className)._result;
        }

        private WindowsByClassFinder(string className)
        {
            _className = className;
            EnumWindows(callback, IntPtr.Zero);
        }

        private bool callback(IntPtr hWnd, IntPtr lparam)
        {
            if (GetClassName(hWnd, _apiResult, _apiResult.Capacity) != 0)
            {
                if (string.CompareOrdinal(_apiResult.ToString(), _className) == 0)
                {
                    _result.Add(hWnd);
                }
            }

            return true; // Keep enumerating.
        }

        public static IEnumerable<string> WindowTitlesForClass(string className)
        {
            foreach (var windowHandle in WindowsMatchingClassName(className))
            {
                int length = GetWindowTextLength(windowHandle);
                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(windowHandle, sb, sb.Capacity);
                yield return sb.ToString();
            }
        }

        public static IEnumerable<IntPtr> WindowsMatchingClassName(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                throw new ArgumentOutOfRangeException("className", className, "className can't be null or blank.");

            return WindowsMatching(className);
        }

        private readonly string _className;
        private readonly List<IntPtr> _result = new List<IntPtr>();
        private readonly StringBuilder _apiResult = new StringBuilder(1024);
    }

    /// <summary>Contains functionality to get all the open windows.</summary>
    public static class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, string> GetOpenWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();
    }
}
