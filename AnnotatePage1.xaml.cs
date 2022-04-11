// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.UI.Xaml.Controls;
using System;
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

        private async void OpenButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            Process p = new();
            string interpreterPath = (string)localSettings.Values["pythonInterpreter"];
            int lastIndex = interpreterPath.LastIndexOf("python");
            if (lastIndex == -1)
                lastIndex = 0;
            string substr = interpreterPath[..lastIndex];
            p.StartInfo.FileName = substr + "labelimg";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            try
            {
                p.Start();//启动进程
            }
            catch (System.ComponentModel.Win32Exception)
            {
                ContentDialog noLabelImgDialog = new()
                {
                    Title = "未找到labelImg",
                    Content = "请检查环境中是否安装labelImg并再试一次。",
                    CloseButtonText = "关闭"
                };
                noLabelImgDialog.XamlRoot = XamlRoot;
                _ = await noLabelImgDialog.ShowAsync();
            }
        }
    }
}
