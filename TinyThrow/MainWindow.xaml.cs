using Microsoft.UI.Dispatching;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;
using Windows.UI.ViewManagement;

namespace TinyThrow;

public sealed partial class MainWindow : WindowEx
{
    private static DispatcherQueue dispatcherQueue;
    private static UISettings uiSettings;
    private readonly IThemeSelectorService _themeSelectorService;
    public static DispatcherQueue MyDispatcherQueue
    {
        get => dispatcherQueue; set => dispatcherQueue = value;
    }

    private void UiSettings_ColorValuesChanged(UISettings sender, object args)
    {
        MyDispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.UpdateTitleBar(_themeSelectorService.Theme);
        });
    }

    public MainWindow()
    {
        InitializeComponent();

        MyDispatcherQueue = DispatcherQueue.GetForCurrentThread();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();

        _themeSelectorService = App.GetService<IThemeSelectorService>();
        uiSettings = new UISettings();
        uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
    }
}
