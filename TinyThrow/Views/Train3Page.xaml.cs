using System.Diagnostics;
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

    public Train3ViewModel ViewModel
    {
        get;
    }
    private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            MainWindow.MyDispatcherQueue.TryEnqueue(() =>
            {
                info.Message += e.Data + "\n";
            });
        }
    }

    private void ProcessExited(object? sender, EventArgs e)
    {
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            info.Message += "Process exited.";
        });
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        parameters = (Parameters)e.Parameter;
        parameter.Text = $"{parameters.Path} {parameters.Folder}\\train.py --img {parameters.Img} --batch {parameters.Batch} --epoch {parameters.Epoch} --data data/throw.yaml --cfg models/{parameters.Model}.yaml --weights weights/{parameters.Model}.pt";
        base.OnNavigatedTo(e);
        LaunchProcess();
        SetTimer();
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
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
        Process p = new();
        //p.StartInfo.WorkingDirectory = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5";
        //p.StartInfo.FileName = parameters.Path; //虚拟环境中python的安装路径
        p.StartInfo.FileName = "python"; //虚拟环境中python的安装路径
                                         //p.StartInfo.Arguments = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolo.py --cfg C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\models\yolov5n6-C3TR-CBAM-P2.yaml";

        // Get the path to the app's Assets folder.
        string root = Environment.CurrentDirectory;
        p.StartInfo.Arguments = "\"" + root + @"\Assets\test.py" + "\"";
        //p.StartInfo.Arguments = @"C:\Users\0Kirby\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\train.py --img 640 --batch 1 --epoch 2 --data data/car.yaml --cfg models/yolov5n.yaml --weights weights/yolov5n.pt";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        //p.StartInfo.CreateNoWindow = true;
        p.EnableRaisingEvents = true;
        p.Exited += new EventHandler(ProcessExited);
        p.Start();//启动进程
        p.BeginErrorReadLine();
        p.ErrorDataReceived += ErrorDataReceived;
        //var time1 = p.StartTime;
        //var time2 = p.ExitTime;
        //var time3 = time2 - time1;
        //info.Title = time3.ToString();
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
            timerText.Text = elapsedTime;
        });
    }
}
