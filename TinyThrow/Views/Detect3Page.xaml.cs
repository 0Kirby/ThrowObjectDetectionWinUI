using Microsoft.UI.Xaml.Controls;

using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Detect3Page : Page
{
    public Detect3ViewModel ViewModel
    {
        get;
    }

    public Detect3Page()
    {
        ViewModel = App.GetService<Detect3ViewModel>();
        InitializeComponent();
    }
}
