// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ThrowObjectDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrainPage1 : Page
    {
        private readonly AppWindow _mainAppWindow = MainWindow.AppWindow;
        public TrainPage1()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.Current.NavView_Navigate("ThrowObjectDetection.TrainPage1", new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }
    }
}
