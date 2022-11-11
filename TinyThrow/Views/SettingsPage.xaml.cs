﻿using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;
using Windows.Storage.Pickers;

namespace TinyThrow.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IThemeSelectorService _themeSelectorService;

    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        _localSettingsService = App.GetService<ILocalSettingsService>();
        _themeSelectorService = App.GetService<IThemeSelectorService>();
        //PythonPath.Text = await LoadPathFromSettingsAsync();
        //PythonPath.Text = _localSettingsService.ReadSettingAsync<string>("pythonInterpreter").ToString();
    }

    private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            MainWindow.MyDispatcherQueue.TryEnqueue(() =>
            {
                SettingsText.Text += e.Data + "\n";
            });
        }
    }

    private async void BrowseBtn_Click(object sender, RoutedEventArgs e)
    {
        FileOpenPicker filePicker = new();
        filePicker.FileTypeFilter.Add(".exe");

        var window = new Microsoft.UI.Xaml.Window();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hWnd);

        //WinRT.Interop.InitializeWithWindow.Initialize(filePicker, _windowHandle);
        var file = await filePicker.PickSingleFileAsync();

        if (file != null)
        {
            //await _localSettingsService.SaveSettingAsync("pythonInterpreter", file.Path);
            await _themeSelectorService.SetPathAsync(file.Path);
            //ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            //localSettings.Values["pythonInterpreter"] = file.Path;
            PythonPath.Text = file.Path;
            ViewModel.Path = file.Path;
        }
        window.Close();
    }

    private void TestBtn_Click(object sender, RoutedEventArgs e)
    {
        SettingsText.Text = "";
        Process p = new();
        //p.StartInfo.WorkingDirectory = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5";
        p.StartInfo.FileName = ViewModel.Path; //虚拟环境中python的安装路径
                                               //p.StartInfo.Arguments = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolo.py --cfg C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolov5n6-C3TR-CBAM-P2.yaml";

        // Get the path to the app's Assets folder.
        string root = Environment.CurrentDirectory;
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
