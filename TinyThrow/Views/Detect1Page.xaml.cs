using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Detect1Page : Page
{
    public Detect1ViewModel ViewModel
    {
        get;
    }

    public Detect1Page()
    {
        ViewModel = App.GetService<Detect1ViewModel>();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        InitializeComponent();
    }

    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo(typeof(Detect2ViewModel).FullName!);
    }

}
