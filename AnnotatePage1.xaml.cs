// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Storage;

namespace ThrowObjectDetection
{
    public partial class AnnotatePage1 : Page
    {
        public AnnotatePage1()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.Current.NavView_SubPage_Navigate("ThrowObjectDetection.AnnotatePage2", new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }
        private void OpenButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            Process p = new Process();
            string interpreterPath = (string)localSettings.Values["pythonInterpreter"];
            int lastIndex = interpreterPath.LastIndexOf("python");
            string substr = interpreterPath.Substring(0, lastIndex);
            p.StartInfo.FileName = substr + "labelimg"; //虚拟环境中python的安装路径
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();//启动进程
        }
    }
}
