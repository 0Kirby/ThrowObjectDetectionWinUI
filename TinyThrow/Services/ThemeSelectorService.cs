using Microsoft.UI.Xaml;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;

namespace TinyThrow.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    private const string SettingsKey = "AppBackgroundRequestedTheme";
    private const string SettingsKey2 = "PythonInterpreter";

    public ElementTheme Theme { get; set; } = ElementTheme.Default;
    public string Path { get; set; } = "python";

    private readonly ILocalSettingsService _localSettingsService;

    public ThemeSelectorService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
        Path = await LoadPathFromSettingsAsync();

        await Task.CompletedTask;
    }

    public async Task SetPathAsync(string path)
    {
        Path = path;

        await SavePathInSettingsAsync(path);
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        var themeName = await _localSettingsService.ReadSettingAsync<string>(SettingsKey);

        if (Enum.TryParse(themeName, out ElementTheme cacheTheme))
        {
            return cacheTheme;
        }

        return ElementTheme.Default;
    }

    private async Task<string> LoadPathFromSettingsAsync()
    {
        var path = await _localSettingsService.ReadSettingAsync<string>(SettingsKey2);

        return path;
    }

    private async Task SaveThemeInSettingsAsync(ElementTheme theme)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey, theme.ToString());
    }

    private async Task SavePathInSettingsAsync(string path)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey2, path);
    }
}
