﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Runtime.InteropServices;
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

        public MainWindow()
        {

            this.InitializeComponent();

            AppWindow = AppWindowExtensions.GetAppWindow(this);
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Window = this;
            Title = Settings.FeatureName;
            HWND hwnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(this);
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            LoadIcon(hwnd, "Assets/windows-sdk.ico");
            SetWindowSize(hwnd, 1050, 800);
            PlacementCenterWindowInMonitorWin32(hwnd);
            //SetDragRegionForCustomTitleBar();
            //UpdateDragRects();
            AppWindow.Changed += MainAppWindow_Changed;
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
        /*
        public static void SetDragRegionForCustomTitleBar()
        {
            //Infer titlebar height
            int titleBarHeight = AppWindow.TitleBar.Height;
            _mainPage = MainPage.Current;
            //_mainPage.MyTitleBar.Height = titleBarHeight;

            // Get caption button occlusion information
            // Use LeftInset if you've explicitly set your window layout to RTL or if app language is a RTL language
            int CaptionButtonOcclusionWidth = AppWindow.TitleBar.RightInset;

            // Define your drag Regions
            int windowIconWidthAndPadding = (int)_mainPage.MyWindowIcon.ActualWidth + (int)_mainPage.MyTitleBar.Margin.Left;
            int dragRegionWidth = AppWindow.Size.Width - (CaptionButtonOcclusionWidth + windowIconWidthAndPadding);

            Windows.Graphics.RectInt32[] dragRects = new Windows.Graphics.RectInt32[] { };
            Windows.Graphics.RectInt32 dragRect;

           
            dragRect.X = windowIconWidthAndPadding;
            dragRect.Y = 0;
            dragRect.Height = 48;
            dragRect.Width =  dragRegionWidth;

            AppWindow.TitleBar.SetDragRectangles(dragRects.Append(dragRect).ToArray());
        }
        */
        private void MainAppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidSizeChange && sender.TitleBar.ExtendsContentIntoTitleBar)
            {
                // Need to update our drag region if the size of the window changes
                //SetDragRegionForCustomTitleBar();
                //UpdateDragRects();
            }
        }
        /*
        private void UpdateDragRects()
        {
            var titleBar = AppWindow.TitleBar;
            _mainPage = MainPage.Current;
            // 当前控件的实际宽度.
            var totalSpace = AppWindow.Size.Width;

            var height = 48;

            // 图标的左边界相对于整个控件左边界的偏移值.
            float iconLeftOffset = _mainPage.MyWindowIcon.ActualOffset.X;
            //MainPage.myTitleText.Text = iconLeftOffset.ToString();
            Console.WriteLine(iconLeftOffset.ToString());
            var dragSpace = totalSpace - iconLeftOffset;

            var Rect = new RectInt32(Convert.ToInt32(iconLeftOffset), 0, Convert.ToInt32(dragSpace), Convert.ToInt32(height));

            titleBar.SetDragRectangles(new RectInt32[] { Rect });
        }
        */
    }
}
