using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class HomePage : Page
{
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IThemeSelectorService _themeSelectorService;
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        _localSettingsService = App.GetService<ILocalSettingsService>();
        _themeSelectorService = App.GetService<IThemeSelectorService>();
        InitializeComponent();
        Initialize();
    }
    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Annotate1ViewModel).FullName!);
    }

    private async void Initialize()
    {
        var interpreterPath = await _localSettingsService.ReadSettingAsync<string>("PythonInterpreter");
        if (interpreterPath is null)
        {
            await _themeSelectorService.SetPathAsync("python");
        }
    }
}
