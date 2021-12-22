﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ThrowObjectDetection
{
    public partial class HomePage : Page
    {
        private AppWindow _mainAppWindow;
        public HomePage()
        {
            this.InitializeComponent();
            _mainAppWindow = MainWindow.AppWindow;
        }

        private async void TitleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox.Text.Contains("\n"))
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Single line text only!",
                    Content = "Only a single line of text is supported for a window titlebar. Please modify your text accordingly.",
                    CloseButtonText = "Ok"
                };
                ContentDialogResult result = await errorDialog.ShowAsync();
            }
            else
            {
                _mainAppWindow.Title = TitleTextBox.Text;
            }
        }

        private void SizeBtn_Click(object sender, RoutedEventArgs e)
        {
            int windowWidth = 0;
            int windowHeight = 0;

            try
            {
                windowWidth = int.Parse(WidthTextBox.Text);
                windowHeight = int.Parse(HeightTextBox.Text);
            }
            catch (FormatException)
            {
                // Silently ignore invalid input...
            }

            if (windowHeight > 0 && windowWidth > 0)
            {
                _mainAppWindow.Resize(new Windows.Graphics.SizeInt32(windowWidth, windowHeight));
            }
        }

        private void Control1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Control1_Unchecked(object sender, RoutedEventArgs e)
        {

        }

    }
}
