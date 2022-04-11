// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;

namespace ThrowObjectDetection
{
    public partial class HomePage : Page
    {
        private readonly AppWindow _mainAppWindow = MainWindow.AppWindow;
        //private readonly MainPage _mainPage = MainPage.Current;
        public HomePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            _mainAppWindow.Changed += MainAppWindow_Changed;
            bool support = Microsoft.UI.Windowing.AppWindowTitleBar.IsCustomizationSupported();
            int dragSpace = MainWindow.CxScreen;
            MainPage.Current.Update(dragSpace);
        }

        private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.Current.NavView_Navigate("ThrowObjectDetection.AnnotatePage1", new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }

        private void MainAppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidSizeChange && sender.TitleBar.ExtendsContentIntoTitleBar)
            {
                // Need to update our drag region if the size of the window changes
                SetDragRegionForCustomTitleBar(sender);
            }
        }

        private static void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {

            int appWindowWidth = appWindow.Size.Width;
            int captionButtonOcclusionWidth = appWindow.TitleBar.RightInset;
            int Space = appWindowWidth - captionButtonOcclusionWidth;
            MainPage.Current.Update(Space);

        }
    }
}
