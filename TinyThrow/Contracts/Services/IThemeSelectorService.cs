using Microsoft.UI.Xaml;

namespace TinyThrow.Contracts.Services;

public interface IThemeSelectorService
{
    ElementTheme Theme
    {
        get;
    }

    string Path
    {
        get;
    }

    Task InitializeAsync();

    Task SetThemeAsync(ElementTheme theme);
    Task SetPathAsync(string path);
    Task SetRequestedThemeAsync();
}
