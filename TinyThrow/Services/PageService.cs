using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;

using TinyThrow.Contracts.Services;
using TinyThrow.ViewModels;
using TinyThrow.Views;

namespace TinyThrow.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<HomeViewModel, HomePage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<Annotate1ViewModel, Annotate1Page>();
        Configure<Annotate2ViewModel, Annotate2Page>();
        Configure<Train1ViewModel, Train1Page>();
        Configure<Train2ViewModel, Train2Page>();
        Configure<Train3ViewModel, Train3Page>();
        Configure<Annotate3ViewModel, Annotate3Page>();
        Configure<Annotate4ViewModel, Annotate4Page>();
        Configure<Detect1ViewModel, Detect1Page>();
        Configure<Detect2ViewModel, Detect2Page>();
        Configure<Detect3ViewModel, Detect3Page>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.Any(p => p.Value == type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
