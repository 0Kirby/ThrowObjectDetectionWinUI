// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

namespace ThrowObjectDetection
{
    public partial class App : Application
    {
        private Window mainWindow;
        private IntPtr _windowHandle;

        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 应用窗口对象.
        /// </summary>
        public static AppWindow AppWindow { get; private set; }

        /// <summary>
        /// 主窗口.
        /// </summary>
        public static Window MainWindow { get; private set; }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            mainWindow = new MainWindow();
            _windowHandle = WindowNative.GetWindowHandle(mainWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(_windowHandle);


            mainWindow.ExtendsContentIntoTitleBar = true;
            //mainWindow.SetTitleBar(MainPage.TitleBar);
            mainWindow.Activate();
        }
    }
}
