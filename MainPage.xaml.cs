// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.Graphics;
using Windows.Storage;

namespace ThrowObjectDetection
{
    public partial class MainPage : Page
    {
        public static MainPage Current;
        public List<Scenario> Scenarios => this.scenarios;

        public MainPage()
        {
            InitializeComponent();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // This is a static public property that allows downstream pages to get a handle to the MainPage instance
            // in order to call methods that are in this class.
            Current = this;
            NavView.IsPaneOpen = false;
            NavView.IsPaneOpen = true;

            InitialLocalSettings();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Scenario item in scenarios)
            {
                NavView.MenuItems.Add(new NavigationViewItem
                {
                    Content = item.Title,
                    Tag = item.ClassName,
                    Icon = new FontIcon() { FontFamily = new("Segoe Fluent Icons"), Glyph = item.Icon }
                });
            }

            NavigationViewItem settings = (NavigationViewItem)NavView.SettingsItem;
            settings.Content = "设置";

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];

            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            if (scenarios is not null && scenarios.Count > 0)
            {
                NavView_Navigate(scenarios.First().ClassName, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            }
        }

        public void NavView_Navigate(string navItemTag, Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type page;

            if (navItemTag == nameof(SettingsPage))
            {
                page = typeof(SettingsPage);
            }
            else
            {
                Scenario item = scenarios.First(p => p.ClassName.Equals(navItemTag));
                page = Type.GetType(item.ClassName);
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if ((page is not null) && !Type.Equals(preNavPageType, page))
            {
                GC.Collect();
                ContentFrame.Navigate(page, null, transitionInfo);
                GC.Collect();
            }
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var naViewItemInvoked = (NavigationViewItem)args.InvokedItemContainer;

            if (args.IsSettingsInvoked)
            {
                NavView_Navigate(nameof(SettingsPage), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer is not null)
            {
                var navItemTag = args.InvokedItemContainer.Tag?.ToString();
                if (!string.IsNullOrEmpty(navItemTag))
                {
                    NavView_Navigate(navItemTag, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
                }
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
                NavView.Header = "设置";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = scenarios.First(p => p.ClassName == e.SourcePageType.FullName);
                var menuItems = NavView.MenuItems;

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.ClassName));

                NavView.Header =
                    ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            }
        }

        private void MyWindowIcon_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Window window = MainWindow.Window;
            window.Close();
        }

        private static void InitialLocalSettings()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            // Auto change themeMode
            object themeMode = localSettings.Values["themeMode"];
            if (themeMode == null)
                localSettings.Values["themeMode"] = 0;
            else
            {
                Grid content = Current.Content as Grid;
                if (content is not null)
                {
                    content.RequestedTheme = (ElementTheme)themeMode;
                    Settings.CurrentTheme = content.RequestedTheme;
                }
            }

            // Initialize Python interpreter
            object pythonInterpreter = localSettings.Values["pythonInterpreter"];
            if (pythonInterpreter == null)
                localSettings.Values["pythonInterpreter"] = "python";
        }

        private void NavigationViewControl_PaneClosing(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_PaneOpened(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness((sender.CompactPaneLength * 2), currMargin.Top, currMargin.Right, currMargin.Bottom);

            }
            else
            {
                AppTitleBar.Margin = new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }

            UpdateAppTitleMargin(sender);
        }

        private void UpdateAppTitleMargin(Microsoft.UI.Xaml.Controls.NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                AppTitle.TranslationTransition = new Vector3Transition();

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Translation = new System.Numerics.Vector3(smallLeftIndent, 0, 0);
                }
                else
                {
                    AppTitle.Translation = new System.Numerics.Vector3(largeLeftIndent, 0, 0);
                }
            }
            else
            {
                Thickness currMargin = AppTitle.Margin;

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            }

            if (sender.DisplayMode != NavigationViewDisplayMode.Minimal)
            {

                Microsoft.UI.Windowing.AppWindowTitleBar titleBar = MainWindow.AppWindow.TitleBar;

                // 当前控件的实际宽度.
                double totalSpace = ActualWidth;
                double height = AppTitleBar.ActualHeight;

                // 搜索框的左边界相对于整个控件左边界的偏移值.

                double dragSpace = AppTitleBar.ActualWidth;

                //bug
                //double leftOffset = totalSpace - dragSpace;

                double leftOffset = 96;
                RectInt32 rect = new RectInt32(Convert.ToInt32(leftOffset), 0, Convert.ToInt32(dragSpace), Convert.ToInt32(height));

                titleBar.SetDragRectangles(new RectInt32[] { rect });
            }
        }
    }
}
