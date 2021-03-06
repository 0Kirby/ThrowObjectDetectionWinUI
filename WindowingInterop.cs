using Microsoft.UI;
using Microsoft.UI.Windowing;
using System;

namespace ThrowObjectDetection
{
    public static class AppWindowExtensions
    {
        public static AppWindow GetAppWindow(this Microsoft.UI.Xaml.Window window)
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            return GetAppWindowFromWindowHandle(windowHandle);
        }

        private static AppWindow GetAppWindowFromWindowHandle(IntPtr windowHandle)
        {
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            return AppWindow.GetFromWindowId(windowId);
        }
    }
}
