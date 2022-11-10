using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;
using Windows.Storage;

namespace TinyThrow.Views;

public sealed partial class Annotate2Page : Page
{
    public Annotate2ViewModel ViewModel
    {
        get;
    }

    private readonly INavigationService navigationService;

    public Annotate2Page()
    {
        ViewModel = App.GetService<Annotate2ViewModel>();
        InitializeComponent();

        navigationService = App.GetService<INavigationService>();
    }

    private void HomeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

        navigationService.NavigateTo(typeof(HomeViewModel).FullName!);
    }

    private void LeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        navigationService.NavigateTo(typeof(Annotate1ViewModel).FullName!);
    }

    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //navigationService.NavigateTo(typeof(Annotate3ViewModel).FullName!);
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
        p.StartInfo.FileName = substr + "labelimg"; //虚拟环境中python的安装路径
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
