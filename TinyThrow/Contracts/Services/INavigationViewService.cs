using Microsoft.UI.Xaml.Controls;

namespace TinyThrow.Contracts.Services;

public interface INavigationViewService
{
    IList<object>? MenuItems
    {
        get;
    }

    object? SettingsItem
    {
        get;
    }

    void Initialize(NavigationView navigationView);

    void UnregisterEvents();

    void DisableNavigationView();

    void EnableNavigationView();

    NavigationViewItem? GetSelectedItem(Type pageType);
}
