using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Train1Page : Page
{
    public Train1ViewModel ViewModel
    {
        get;
    }

    public Train1Page()
    {
        ViewModel = App.GetService<Train1ViewModel>();
        this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        InitializeComponent();
    }

    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Train2ViewModel).FullName!);
    }

}
