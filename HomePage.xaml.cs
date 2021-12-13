// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Windows.Storage;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WinRT;
using Windows.Win32.Foundation;

namespace ThrowObjectDetection
{
    public partial class HomePage : Page
    {
        AppWindow m_mainAppWindow;
        HWND m_windowHandle;
        public HomePage()
        {
       
            this.InitializeComponent();
            Window window = MainWindow.Current;
            m_windowHandle = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(window);
            m_mainAppWindow = AppWindowExtensions.GetAppWindow(window);
            
        }

        private async void TitleBtn_Click(object sender, RoutedEventArgs e)
        {
            //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(m_mainAppWindow);
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, m_windowHandle);
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
                m_mainAppWindow.Title = TitleTextBox.Text;
            }

            if (file != null)
            {
                m_mainAppWindow.Title = "!!!";
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
                m_mainAppWindow.Resize(new Windows.Graphics.SizeInt32(windowWidth, windowHeight));
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
