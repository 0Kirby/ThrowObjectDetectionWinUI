using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Annotate4Page : Page
{
    private readonly ILocalSettingsService _localSettingsService;
    private readonly INavigationService navigationService;
    public Annotate4ViewModel ViewModel
    {
        get;
    }

    public Annotate4Page()
    {
        ViewModel = App.GetService<Annotate4ViewModel>();
        InitializeComponent();
        _localSettingsService = App.GetService<ILocalSettingsService>();
        navigationService = App.GetService<INavigationService>();
    }

    private void HomeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

        navigationService.NavigateTo(typeof(Annotate1ViewModel).FullName!);
    }

    private void LeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        navigationService.NavigateTo(typeof(Annotate3ViewModel).FullName!);
    }

    private async void OpenButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        int lastIndex;
        string substr = "";
        var interpreterPath = await _localSettingsService.ReadSettingAsync<string>("PythonInterpreter");
        Process p = new();
        if (interpreterPath is not null)
        {
            lastIndex = interpreterPath.LastIndexOf("python");
            if (lastIndex != -1)
            {
                substr = interpreterPath[..lastIndex];
            }
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
                Title = "未找到LabelImg",
                Content = "请检查环境中是否安装LabelImg并再试一次。",
                CloseButtonText = "关闭",
                XamlRoot = XamlRoot
            };
            _ = await noLabelImgDialog.ShowAsync();
        }
    }
}
