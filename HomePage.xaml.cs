// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Win32.Foundation;

namespace ThrowObjectDetection
{
    public partial class HomePage : Page
    {
        private AppWindow _mainAppWindow;
        private Window _mainWindow;
        private HWND _windowHandle;
        public HomePage()
        {

            this.InitializeComponent();

            _mainAppWindow = MainWindow.AppWindow;
            _mainWindow = MainWindow.Window;
            _windowHandle = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(_mainWindow);
        }

        private async void TitleBtn_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, _windowHandle);
            var file = await filePicker.PickSingleFileAsync();

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

            if (file != null)
            {
                _mainAppWindow.Title = "!!!";
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
