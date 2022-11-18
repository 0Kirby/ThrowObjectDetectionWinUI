using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Timers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;
using TinyThrow.ViewModels;

namespace TinyThrow.Views;

public sealed partial class Train3Page : Page
{
    private Parameters parameters;
    private static System.Timers.Timer aTimer;
    private Stopwatch _stopwatch;
    private Process p;
    private DateTime startTime;

    public Train3ViewModel ViewModel
    {
        get;
    }

    private void ProcessExited(object? sender, EventArgs e)
    {
        aTimer.Stop();
        _stopwatch.Stop();
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            ContentDialog launchComplete = new()
            {
                Title = "提示",
                Content = "运行成功！",
                CloseButtonText = "关闭"
            };
            launchComplete.XamlRoot = XamlRoot;
            launchComplete.ShowAsync();
            stopTrain.IsEnabled = false;
            home.IsEnabled = true;
            left.IsEnabled = true;

            TimeSpan ts = p.ExitTime - startTime;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            timerText.Text = startTime + "    |    " + p.ExitTime + "    |    " + elapsedTime;
        });
        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        parameters = (Parameters)e.Parameter;
        parameter.Text = $"{parameters.Path} {parameters.Folder}\\train.py --img {parameters.Img} --batch {parameters.Batch} --epoch {parameters.Epoch} --data data/throw.yaml --cfg models/{parameters.Model}.yaml --weights weights/{parameters.Model}.pt";
        FindDirectories();
        stopTrain.IsEnabled = true;
        home.IsEnabled = false;
        left.IsEnabled = false;
        LaunchProcess();
        SetTimer();
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        base.OnNavigatedTo(e);
    }

    private void FindDirectories()
    {
        try
        {
            var dirs = Directory.GetDirectories(parameters.Folder + @"\runs\train\", "exp*", SearchOption.TopDirectoryOnly);
            int max = 0;
            string r = @"[0-9]+";
            Regex regex = new Regex(r, RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromSeconds(3)); //2秒后超时       
            foreach (var dir in dirs)
            {
                var dirName = dir.Split('\\').Last();
                MatchCollection matchCollection = regex.Matches(dirName);
                if (matchCollection.Count > 0)
                {
                    var num = int.Parse(matchCollection[0].Value);
                    if (num > max)
                    {
                        max = num;
                    }
                }
                else
                    max = 1;
                //var numberString = System.Text.RegularExpressions.Regex.Replace(dirName, @"[^0-9]+", "");
            }
            if (max != 0)
                output.Text = parameters.Folder + @"\runs\train\exp" + (max + 1);
            else
                output.Text = parameters.Folder + @"\runs\train\exp";
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            Directory.CreateDirectory(parameters.Folder + @"\runs\train\");
            output.Text = parameters.Folder + @"\runs\train\exp";
        }
    }
    public Train3Page()
    {
        ViewModel = App.GetService<Train3ViewModel>();
        this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
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

        navigationService.NavigateTo(typeof(Train2ViewModel).FullName!);
    }

    private void LaunchProcess()
    {
        p = new();
        p.StartInfo.WorkingDirectory = parameters.Folder;
        p.StartInfo.FileName = parameters.Path; //虚拟环境中python的安装路径
        // Get the path to the app's Assets folder.
        string root = Environment.CurrentDirectory;
        //p.StartInfo.Arguments = @"C:\Users\jty\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\train.py --img 640 --batch 4 --epoch 1 --data data/throw.yaml --cfg models/yolov5n.yaml --weights weights/yolov5n.pt";
        p.StartInfo.Arguments = $"{parameters.Folder}\\train.py --img {parameters.Img} --batch {parameters.Batch} --epoch {parameters.Epoch} --data data/throw.yaml --cfg models/yolov5n.yaml --weights weights/yolov5n.pt";
        p.StartInfo.UseShellExecute = false;
        p.EnableRaisingEvents = true;
        p.Exited += new EventHandler(ProcessExited);
        p.Start();//启动进程
        startTime = p.StartTime;
    }

    private void SetTimer()
    {
        // Create a timer with a two second interval.
        aTimer = new System.Timers.Timer(1000);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            //timerText.Text = e.SignalTime.ToString();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = _stopwatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            timerText.Text = startTime + @"    |    ----/--/-- --:--:--    |    " + elapsedTime;
        });
    }

    private void stopTrain_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        p.Kill();
    }
}
