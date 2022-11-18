using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;
using TinyThrow.ViewModels;
using Windows.Storage.Pickers;

namespace TinyThrow.Views;

public sealed partial class Train2Page : Page
{

    public Train2ViewModel ViewModel
    {
        get;
    }

    public SettingsViewModel ViewModel2
    {
        get; set;
    }

    public Train2Page()
    {
        ViewModel = App.GetService<Train2ViewModel>();
        ViewModel2 = App.GetService<SettingsViewModel>();
        this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        InitializeComponent();
    }

    private void HomeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Train1ViewModel).FullName!);
    }
    private void LeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Train1ViewModel).FullName!);
    }
    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel2 = App.GetService<SettingsViewModel>();
        Parameters parameters = new Parameters(ViewModel2.Path, ScriptText.Text, imgsz.Text, batch.Text, epoch.Text, "YOLOv5");

        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Train3ViewModel).FullName!, parameters);
    }
    private void SettingsButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }
    private async void BrowseBtn1_Click(object sender, RoutedEventArgs e)
    {
        FolderPicker folderPicker = new();

        var window = new Microsoft.UI.Xaml.Window();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);
        var file = await folderPicker.PickSingleFolderAsync();

        if (file != null)
        {
            DatasetText.Text = file.Path;
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
            ScriptText.Text = file.Path;
        }
        window.Close();
    }
}
