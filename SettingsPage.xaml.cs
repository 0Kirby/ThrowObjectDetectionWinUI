// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Win32.Foundation;

namespace ThrowObjectDetection
{
    public partial class SettingsPage : Page
    {
        private Window _mainWindow;
        private HWND _windowHandle;

        public SettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            PythonPath.Text = (String)localSettings.Values["pythonInterpreter"];
            _mainWindow = MainWindow.Window;
            _windowHandle = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(_mainWindow);

        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                MainWindow.dispatcherQueue.TryEnqueue(() =>
                {
                    SettingsText.Text += e.Data + "\n";
                });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            themePanel.Children.Cast<RadioButton>().First(c => (ElementTheme)c.Tag == Settings.CurrentTheme).IsChecked = true;
            base.OnNavigatedTo(e);
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            ElementTheme selectedTheme = (ElementTheme)((RadioButton)sender).Tag;
            Grid content = MainPage.Current.Content as Grid;
            if (content is not null)
            {
                content.RequestedTheme = selectedTheme;
                Settings.CurrentTheme = content.RequestedTheme;
                localSettings.Values["themeMode"] = (int)selectedTheme;
                switch ((int)selectedTheme)
                {
                    case 0:
                        MainWindow.UpdateSystemCaptionButtonColors();
                        break;
                    case 1:
                        MainWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                        break;
                    case 2:
                        MainWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                        break;
                }
            }
        }

        private async void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".exe");
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, _windowHandle);
            var file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["pythonInterpreter"] = file.Path;
                PythonPath.Text = file.Path;
            }
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsText.Text = "";
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            Process p = new Process();
            p.StartInfo.WorkingDirectory = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5";
            p.StartInfo.FileName = (String)localSettings.Values["pythonInterpreter"]; //虚拟环境中python的安装路径
            //p.StartInfo.Arguments = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolo.py --cfg C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolov5n6-C3TR-CBAM-P2.yaml";

            // Get the path to the app's Assets folder.
            string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            p.StartInfo.Arguments = "\"" + root + @"\Assets\test.py" + "\"";
            //p.StartInfo.Arguments = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\train.py --img 640 --batch 1 --epoch 2 --data data/car.yaml --cfg models/yolov5n.yaml --weights weights/yolov5n.pt";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();//启动进程
            p.BeginErrorReadLine();
            p.ErrorDataReceived += ErrorDataReceived;
        }
    }
}
