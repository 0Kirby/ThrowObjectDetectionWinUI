using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Annotate1Page : Page
{
    private readonly ILocalSettingsService _localSettingsService;

    public Annotate1ViewModel ViewModel
    {
        get;
    }

    public Annotate1Page()
    {
        ViewModel = App.GetService<Annotate1ViewModel>();
        InitializeComponent();
        _localSettingsService = App.GetService<ILocalSettingsService>();
    }

    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Annotate2ViewModel).FullName!);
    }

    private async void OpenButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        int lastIndex;
        string substr;
        await _localSettingsService.SaveSettingAsync<string>("pythonInterpreter", "python.exe");
        var interpreterPath = await _localSettingsService.ReadSettingAsync<string>("pythonInterpreter");
        //ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        Process p = new();
        //string interpreterPath = (string)localSettings.Values["pythonInterpreter"];
        if (interpreterPath is not null)
        {
            lastIndex = interpreterPath.LastIndexOf("python");
            substr = interpreterPath[..lastIndex];
        }
        else
        {
            substr = "";
        }

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
