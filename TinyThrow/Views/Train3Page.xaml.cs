using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Train3Page : Page
{
    public Train3ViewModel ViewModel
    {
        get;
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Parameters parameters = (Parameters)e.Parameter;
        parameter.Text = $"{parameters.Path}\\train.py --img {parameters.Img} --batch {parameters.Batch} --epoch {parameters.Epoch} --data data/throw.yaml --cfg models/{parameters.Model}.yaml --weights weights/{parameters.Model}.pt";
        base.OnNavigatedTo(e);
    }
    public Train3Page()
    {
        ViewModel = App.GetService<Train3ViewModel>();
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

        navigationService.NavigateTo(typeof(Train2ViewModel).FullName!);
    }
}
