﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace ThrowObjectDetection
{
    public partial class MainPage : Page
    {
        public static MainPage Current;
        Window window;
        public List<Scenario> Scenarios => this.scenarios;

        public MainPage()
        {
            window = MainWindow.Current;
            InitializeComponent();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // This is a static public property that allows downstream pages to get a handle to the MainPage instance
            // in order to call methods that are in this class.
            Current = this;

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
                    Icon = new FontIcon() { FontFamily = new("Segoe MDL2 Assets"), Glyph = item.Icon }
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

        private void NavView_Navigate(string navItemTag, Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
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
    }
}
