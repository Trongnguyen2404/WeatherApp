using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinRT.Interop;

namespace WeatherApp.Platforms.Windows
{
    public static class WindowService
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x00010000;

        public static void RemoveMaximizeButton(Window window)
        {
            var hWnd = WindowNative.GetWindowHandle(window);
            var style = GetWindowLong(hWnd, GWL_STYLE);
            style &= ~WS_MAXIMIZEBOX;
            SetWindowLong(hWnd, GWL_STYLE, style);
        }
    }
}
