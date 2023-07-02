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
