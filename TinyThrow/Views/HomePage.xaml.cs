using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();
    }
    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Annotate1ViewModel).FullName!);

    }
}
