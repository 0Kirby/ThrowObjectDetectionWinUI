using Microsoft.UI.Xaml.Controls;

using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Detect2Page : Page
{
    public Detect2ViewModel ViewModel
    {
        get;
    }

    public Detect2Page()
    {
        ViewModel = App.GetService<Detect2ViewModel>();
        InitializeComponent();
    }
}
