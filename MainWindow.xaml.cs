// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace ThrowObjectDetection
{
    public partial class MainWindow : Window
    {
        public static AppWindow AppWindow;
        public static DispatcherQueue dispatcherQueue;
        public static Window Window;
        public string WindowTitle;
        public static MainPage _mainPage;
        private static UISettings uiSettings;

        public MainWindow()
        {

            this.InitializeComponent();

            AppWindow = AppWindowExtensions.GetAppWindow(this);
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            int savedThemeMode = (int)localSettings.Values["themeMode"];

            switch (savedThemeMode)
            {
                case 0:
                    UpdateSystemCaptionButtonColors();
                    break;
                case 1:
                    AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                    break;
                case 2:
                    AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                    break;
            }

            //MainPage
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Window = this;

            Title = Settings.FeatureName;
            HWND hwnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(this);
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            LoadIcon(hwnd, "Assets/windows-sdk.ico");
            SetWindowSize(hwnd, 1050, 800);
            PlacementCenterWindowInMonitorWin32(hwnd);

            uiSettings = new UISettings();
            uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
        }

        private unsafe void LoadIcon(HWND hwnd, string iconName)
        {
            const int ICON_SMALL = 0;
            const int ICON_BIG = 1;

            fixed (char* nameLocal = iconName)
            {
                HANDLE imageHandle = LoadImage(default,
                    nameLocal,
                    GDI_IMAGE_TYPE.IMAGE_ICON,
                    GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSMICON),
                    GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSMICON),
                    IMAGE_FLAGS.LR_LOADFROMFILE | IMAGE_FLAGS.LR_SHARED);
                SendMessage(hwnd, WM_SETICON, ICON_SMALL, imageHandle.Value);
            }

            fixed (char* nameLocal = iconName)
            {
                HANDLE imageHandle = LoadImage(default,
                    nameLocal,
                    GDI_IMAGE_TYPE.IMAGE_ICON,
                    GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSMICON),
                    GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSMICON),
                    IMAGE_FLAGS.LR_LOADFROMFILE | IMAGE_FLAGS.LR_SHARED);
                SendMessage(hwnd, WM_SETICON, ICON_BIG, imageHandle.Value);
            }
        }

        private void SetWindowSize(HWND hwnd, int width, int height)
        {
            // Win32 uses pixels and WinUI 3 uses effective pixels, so you should apply the DPI scale factor
            uint dpi = GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            SetWindowPos(hwnd, default, 0, 0, width, height, SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
        }

        private void PlacementCenterWindowInMonitorWin32(HWND hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);
            ClipOrCenterRectToMonitorWin32(ref rc);
            SetWindowPos(hwnd, default, rc.left, rc.top, 0, 0,
                         SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                         SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                         SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
        }

        private void ClipOrCenterRectToMonitorWin32(ref RECT prc)
        {
            HMONITOR hMonitor;
            RECT rc;
            int w = prc.right - prc.left;
            int h = prc.bottom - prc.top;

            hMonitor = MonitorFromRect(prc, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
            MONITORINFO mi = new MONITORINFO();
            mi.cbSize = (uint)Marshal.SizeOf<MONITORINFO>();

            GetMonitorInfo(hMonitor, ref mi);

            rc = mi.rcWork;
            prc.left = rc.left + (rc.right - rc.left - w) / 2;
            prc.top = rc.top + (rc.bottom - rc.top - h) / 2;
            prc.right = prc.left + w;
            prc.bottom = prc.top + h;
        }

        private void MyWindowIcon_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.Close();
        }

        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            // Make sure we have a reference to our window so we dispatch a UI change
            // Dispatch on UI thread so that we have a current appbar to access and change
            dispatcherQueue.TryEnqueue(() =>
            {
                UpdateSystemCaptionButtonColors();
            });

        }

        public static void UpdateSystemCaptionButtonColors()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            int savedThemeMode = (int)localSettings.Values["themeMode"];
            var themeMode = Application.Current.RequestedTheme;
            if (savedThemeMode == 0)
                if (themeMode == ApplicationTheme.Dark)
                    AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                else
                    AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
        }

    }
}
