using Microsoft.UI.Dispatching;
using TinyThrow.Helpers;

namespace TinyThrow;

public sealed partial class MainWindow : WindowEx
{
    private static DispatcherQueue dispatcherQueue;
    public static DispatcherQueue MyDispatcherQueue
    {
        get => dispatcherQueue; set => dispatcherQueue = value;
    }
    public MainWindow()
    {
        InitializeComponent();

        MyDispatcherQueue = DispatcherQueue.GetForCurrentThread();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
    }
}
