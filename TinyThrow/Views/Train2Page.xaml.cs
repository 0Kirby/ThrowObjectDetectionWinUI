using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;
using TinyThrow.ViewModels;
using Windows.Storage.Pickers;

namespace TinyThrow.Views;

public sealed partial class Train2Page : Page
{
    private readonly string[] depth_multiple = { "0.33", "0.33", "0.67", "1.0", "1.33" };
    private readonly string[] width_multiple = { "0.25", "0.50", "0.75", "1.0", "1.25" };

    public Train2ViewModel ViewModel
    {
        get;
    }

    public SettingsViewModel ViewModel2
    {
        get; set;
    }

    public Train2Page()
    {
        ViewModel = App.GetService<Train2ViewModel>();
        ViewModel2 = App.GetService<SettingsViewModel>();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        InitializeComponent();
    }

    private void HomeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Train1ViewModel).FullName!);
    }
    private void LeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Train1ViewModel).FullName!);
    }
    private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel2 = App.GetService<SettingsViewModel>();
        Parameters parameters = new Parameters(ViewModel2.Path, ScriptText.Text, imgsz.Text, batch.Text, epoch.Text, (string)model.SelectedItem, (string)scale.SelectedItem, device.Text);

        //生成指定数据集路径的配置文件
        string[] output = ReadYaml(ScriptText.Text + "/data/dataset.yaml", 10, 1);
        if (output[0] != null)
        {
            output[1] = "path: " + DatasetText.Text;
            WriteYaml(ScriptText.Text + "/data/dataset_new.yaml", output);

            //生成自定义算法规模的配置文件
            string path2 = "";
            if (model.SelectedItem.ToString() == "TinyThrow")
            {
                path2 = ScriptText.Text + "/models/TinyThrow.yaml";
            }
            else
            {
                path2 = ScriptText.Text + "/models/YOLOv5.yaml";
            }

            output = ReadYaml(path2, 4, 2);

            string depth = "";
            string width = "";

            switch ((string)scale.SelectedItem)
            {
                case "Nano":
                    depth = depth_multiple[0];
                    width = width_multiple[0];
                    break;
                case "Small":
                    depth = depth_multiple[1];
                    width = width_multiple[1];
                    break;
                case "Medium":
                    depth = depth_multiple[2];
                    width = width_multiple[2];
                    break;
                case "Large":
                    depth = depth_multiple[3];
                    width = width_multiple[3];
                    break;
                case "ExtraLarge":
                    depth = depth_multiple[4];
                    width = width_multiple[4];
                    break;
            }

            output[1] = "depth_multiple: " + depth + "  # model depth multiple\n" + "width_multiple: " + width + "  # layer channel multiple";
            WriteYaml(ScriptText.Text + "/models/script.yaml", output);

            var navigationService = App.GetService<INavigationService>();

            navigationService.NavigateTo(typeof(Train3ViewModel).FullName!, parameters);
        }
    }
    private void SettingsButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }
    private async void BrowseBtn1_Click(object sender, RoutedEventArgs e)
    {
        FolderPicker folderPicker = new();

        var window = new Microsoft.UI.Xaml.Window();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);
        var file = await folderPicker.PickSingleFolderAsync();

        if (file != null)
        {
            DatasetText.Text = file.Path;
        }
        window.Close();
    }
    private async void BrowseBtn2_Click(object sender, RoutedEventArgs e)
    {
        FolderPicker folderPicker = new();

        var window = new Microsoft.UI.Xaml.Window();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);
        var file = await folderPicker.PickSingleFolderAsync();

        if (file != null)
        {
            ScriptText.Text = file.Path;
        }
        window.Close();
    }

    private void Button_Enable_Text(object sender, TextChangedEventArgs e)
    {
        if (imgsz.Text != "" && batch.Text != "" && epoch.Text != "" && model.SelectedItem != null && scale.SelectedItem != null
            && device.Text != "" && DatasetText.Text != "" && ScriptText.Text != "")
        {
            RightButton.IsEnabled = true;
            start.IsEnabled = true;
        }
        else
        {
            RightButton.IsEnabled = false;
            start.IsEnabled = false;
        }
    }

    private void Button_Enable_Selection(object sender, SelectionChangedEventArgs e)
    {
        if (imgsz.Text != "" && batch.Text != "" && epoch.Text != "" && model.SelectedItem != null && scale.SelectedItem != null
            && device.Text != "" && DatasetText.Text != "" && ScriptText.Text != "")
        {
            cpuOnly.IsOn = false;
            RightButton.IsEnabled = true;
            start.IsEnabled = true;
        }
        else
        {
            RightButton.IsEnabled = false;
            start.IsEnabled = false;
        }
    }

    private void Button_Enable_Toggle(object sender, RoutedEventArgs e)
    {
        if (cpuOnly.IsOn == true)
        {
            device.Text = "cpu";
            device.IsEnabled = false;
        }
        else
        {
            device.Text = "0";
            device.IsEnabled = true;
        }
    }

    //连续读文本,分离出需要修改的行
    private string[] ReadYaml(string path, int lines, int repeat)
    {
        string[] text = new string[3];
        try
        {
            StreamReader streamReader = new StreamReader(path);

            string text1 = "";
            string text2 = "";

            for (int i = 0; i < lines; i++)
            {
                text1 += streamReader.ReadLine();
                if (i != lines - 1)
                {
                    text1 += "\n";
                }
            }

            for (int i = 0; i < repeat; i++)
            {
                text2 += streamReader.ReadLine();
                if (i != repeat - 1)
                {
                    text2 += "\n";
                }
            }

            string text3 = streamReader.ReadToEnd();
            streamReader.Close();
            text[0] = text1;
            text[1] = text2;
            text[2] = text3;
        }

        catch (Exception)
        {
            DisplayFileNotFoundDialog();
        }
        return text;
    }

    //将文本写入文件
    private void WriteYaml(string path, string[] text)
    {
        StreamWriter streamWriter = new StreamWriter(path);
        streamWriter.WriteLine(text[0]);
        streamWriter.WriteLine(text[1]);
        streamWriter.Write(text[2]);
        streamWriter.Close();
    }

    private void DisplayFileNotFoundDialog()
    {
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            ContentDialog noFileDialog = new ContentDialog
            {
                Title = "警告",
                Content = "未找到算法需要的文件，请确认路径是否正确！",
                CloseButtonText = "关闭",
                XamlRoot = XamlRoot
            };
            noFileDialog.ShowAsync();
        });
    }
}
