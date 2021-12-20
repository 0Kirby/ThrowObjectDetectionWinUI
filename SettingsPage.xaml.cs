// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;

namespace ThrowObjectDetection
{
    public partial class SettingsPage : Page
    {
        public delegate void DelReadErrOutput(string result);

        public SettingsPage()
        {
            this.InitializeComponent();
            RunPythonCode();
        }

        private void RunPythonCode()
        {
            SettingsText.Text = "";
            Process p = new Process();
            p.StartInfo.WorkingDirectory = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5";
            p.StartInfo.FileName = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\venv\Scripts\python.exe"; //虚拟环境中python的安装路径
            //p.StartInfo.Arguments = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolo.py --cfg C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolov5n6-C3TR-CBAM-P2.yaml";
            p.StartInfo.Arguments = @"D:\hello.py";
            //p.StartInfo.Arguments = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\train.py --img 640 --batch 1 --epoch 2 --data data/car.yaml --cfg models/yolov5n.yaml --weights weights/yolov5n.pt";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();//启动进程
            p.BeginErrorReadLine();
            p.ErrorDataReceived += ErrorDataReceived;

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
            }
        }
    }
}
