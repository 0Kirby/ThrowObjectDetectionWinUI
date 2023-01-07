using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;
using TinyThrow.ViewModels;
using Windows.Storage.Pickers;

namespace TinyThrow.Views;

public sealed partial class Detect2Page : Page
{

    public Detect2ViewModel ViewModel
    {
        get;
    }

    public SettingsViewModel ViewModel2
    {
        get; set;
    }

    public Detect2Page()
    {
        ViewModel = App.GetService<Detect2ViewModel>();
        ViewModel2 = App.GetService<SettingsViewModel>();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        InitializeComponent();
    }

    private void HomeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Detect1ViewModel).FullName!);
    }
    private void LeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Detect1ViewModel).FullName!);
    }
    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel2 = App.GetService<SettingsViewModel>();
        Parameters parameters = new Parameters(ViewModel2.Path, scriptText.Text, imgsz.Text, device.Text, augment.IsOn, half.IsOn, weightsText.Text, datasetText.Text);

        if (CheckPath(scriptText.Text + "/data/dataset.yaml"))
        {
            var navigationService = App.GetService<INavigationService>();

            navigationService.NavigateTo(typeof(Detect3ViewModel).FullName!, parameters);
        }
    }

    private void SettingsButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }
    private async void BrowseBtn1_Click(object sender, RoutedEventArgs e)
    {
        FileOpenPicker fileOpenPicker = new();

        var window = new Microsoft.UI.Xaml.Window();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hWnd);
        fileOpenPicker.FileTypeFilter.Add(".pt");
        var file = await fileOpenPicker.PickSingleFileAsync();

        if (file != null)
        {
            weightsText.Text = file.Path;
        }
        window.Close();
    }

    private async void BrowseBtn2_Click(object sender, RoutedEventArgs e)
    {
        FolderPicker folderPicker = new();

        var window = new Microsoft.UI.Xaml.Window();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);
        var file = await folderPicker.PickSingleFolderAsync();

        if (file != null)
        {
            scriptText.Text = file.Path;
        }
        window.Close();
    }
    private async void BrowseBtn3_Click(object sender, RoutedEventArgs e)
    {
        FolderPicker folderPicker = new();

        var window = new Microsoft.UI.Xaml.Window();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);
        var file = await folderPicker.PickSingleFolderAsync();

        if (file != null)
        {
            datasetText.Text = file.Path;
        }
        window.Close();
    }

    private void Button_Enable_Text(object sender, TextChangedEventArgs e)
    {
        if (imgsz.Text != "" && device.Text != "" && weightsText.Text != "" && scriptText.Text != "" && datasetText.Text != "")
        {
            RightButton.IsEnabled = true;
            start.IsEnabled = true;
        }
        else
        {
            RightButton.IsEnabled = false;
            start.IsEnabled = false;
        }
    }

    private void Button_Enable_Toggle(object sender, RoutedEventArgs e)
    {
        if (cpuOnly.IsOn == true)
        {
            device.Text = "cpu";
            device.IsEnabled = false;
        }
        else
        {
            device.Text = "0";
            device.IsEnabled = true;
        }
    }

    private bool CheckPath(string path)
    {
        try
        {
            StreamReader streamReader = new StreamReader(path);
            return true;
        }
        catch (Exception)
        {
            DisplayFileNotFoundDialog();
            return false;
        }
    }

    private void DisplayFileNotFoundDialog()
    {
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            ContentDialog noFileDialog = new ContentDialog
            {
                Title = "警告",
                Content = "未找到算法需要的文件，请确认路径是否正确！",
                CloseButtonText = "关闭",
                XamlRoot = XamlRoot
            };
            _ = noFileDialog.ShowAsync();
        });
    }
}
