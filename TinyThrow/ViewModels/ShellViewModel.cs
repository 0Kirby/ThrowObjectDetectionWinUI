using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Navigation;

using TinyThrow.Contracts.Services;
using TinyThrow.Views;

namespace TinyThrow.ViewModels;

public class ShellViewModel : ObservableRecipient
{
    private bool _isBackEnabled;
    private object? _selected;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public bool IsBackEnabled
    {
        get => _isBackEnabled;
        set => SetProperty(ref _isBackEnabled, value);
    }

    public object? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;
            return;
        }

        if (NavigationViewService.MenuItems != null)
        {
            if (e.SourcePageType == typeof(Annotate2Page) || e.SourcePageType == typeof(Annotate3Page) || e.SourcePageType == typeof(Annotate4Page))
            {
                Selected = NavigationViewService.MenuItems[2];
                return;
            }

            if (e.SourcePageType == typeof(Train2Page) || e.SourcePageType == typeof(Train3Page))
            {
                Selected = NavigationViewService.MenuItems[3];
                return;
            }

            if (e.SourcePageType == typeof(Detect2Page) || e.SourcePageType == typeof(Detect3Page))
            {
                Selected = NavigationViewService.MenuItems[4];
                return;
            }
        }

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }
}
