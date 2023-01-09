using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Timers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using TinyThrow.Contracts.Services;
using TinyThrow.Helpers;
using TinyThrow.ViewModels;
using Windows.ApplicationModel.DataTransfer;

namespace TinyThrow.Views;

public sealed partial class Detect3Page : Page
{
    private Parameters parameters;
    private static System.Timers.Timer aTimer;
    private Stopwatch _stopwatch;
    private Process p;
    private DateTime startTime;
    private Run run;
    private Hyperlink hyperlink;
    private readonly ShellViewModel shellViewModel;
    private bool tipPoped = false;

    public Detect3ViewModel ViewModel
    {
        get;
    }

    public Detect3Page()
    {
        ViewModel = App.GetService<Detect3ViewModel>();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        InitializeComponent();
        shellViewModel = ShellPage.ViewModelPublic;
    }

    private void ProcessExited(object? sender, EventArgs e)
    {
        aTimer.Stop();
        _stopwatch.Stop();

        string content = "";
        string notification = "";

        if (p.ExitCode == 0)
        {
            content = "运行成功！";
            notification = "AppNotificationSuccess";
        }
        else
        {
            content = "运行失败！";
            notification = "AppNotificationFailure";
        }

        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            ContentDialog launchComplete = new()
            {
                Title = "提示",
                Content = content,
                CloseButtonText = "关闭",
                XamlRoot = XamlRoot
            };
            _ = launchComplete.ShowAsync();
            stopDetect.IsEnabled = false;
            home.IsEnabled = true;
            left.IsEnabled = true;
            progressRing.IsActive = false;
            bottomProgressRing.IsActive = false;
            autoScroll.IsEnabled = false;
            shellViewModel.NavigationViewService.EnableNavigationView();

            TimeSpan ts = p.ExitTime - startTime;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            timerText.Text = startTime + "    |    " + p.ExitTime + "    |    " + elapsedTime;
            bottomTimerText.Text = elapsedTime;
        });
        App.GetService<IAppNotificationService>().Show(string.Format(notification.GetLocalized(), AppContext.BaseDirectory));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        parameters = (Parameters)e.Parameter;

        parameter.Text = $"{parameters.Path} {parameters.Folder}\\detect.py --img {parameters.Img} --weights {parameters.Weights}" +
            $" --source {parameters.Dataset}\\images\\test --device {parameters.Device}";

        if (parameters.Augment)
        {
            parameter.Text += " --augment";
        }

        if (parameters.Half)
        {
            parameter.Text += " --half";
        }

        output.Inlines.Clear();
        FindDirectories();
        stopDetect.IsEnabled = true;
        home.IsEnabled = false;
        left.IsEnabled = false;
        progressRing.IsActive = true;
        bottomProgressRing.IsActive = true;
        timerText.Text = "----/--/-- --:--:--    |    ----/--/-- --:--:--    |    00:00:00";
        bottomTimerText.Text = "00:00:00";
        outputText.Text = "";
        autoScroll.IsEnabled = true;
        autoScroll.IsChecked = true;
        shellViewModel.NavigationViewService.DisableNavigationView();
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
            run = new Run();
            hyperlink = new Hyperlink();
            hyperlink.Click += OpenOutputFolder;

            var dirs = Directory.GetDirectories(parameters.Folder + @"\runs\detect\", "exp*", SearchOption.TopDirectoryOnly);
            int max = 0;
            string r = @"[0-9]+";
            Regex regex = new Regex(r, RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromSeconds(3)); //3秒后超时       
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
                run.Text = parameters.Folder + @"\runs\detect\exp" + (max + 1);
            else
                run.Text = parameters.Folder + @"\runs\detect\exp";
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            Directory.CreateDirectory(parameters.Folder + @"\runs\detect\");
            run.Text = parameters.Folder + @"\runs\detect\exp";
        }
        finally
        {
            hyperlink.Inlines.Add(run);
            output.Inlines.Add(hyperlink);
        }
    }

    private void HomeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo(typeof(Detect1ViewModel).FullName!);
    }
    private void LeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.NavigateTo(typeof(Detect2ViewModel).FullName!);
    }

    private void LaunchProcess()
    {
        p = new();
        p.StartInfo.WorkingDirectory = parameters.Folder;
        p.StartInfo.FileName = parameters.Path; //虚拟环境中python的安装路径
        // Get the path to the app's Assets folder.
        string root = Environment.CurrentDirectory;
        //p.StartInfo.Arguments = @"C:\Users\jty\PycharmProjects\MachineLearning\CarObjectDetectionTest\yolov5\train.py --img 640 --batch 4 --epoch 1 --data data/throw.yaml --cfg models/yolov5n.yaml --weights weights/yolov5n.pt";
        p.StartInfo.Arguments = $"{parameters.Folder}\\detect.py --img {parameters.Img} --weights {parameters.Weights}" +
            $" --source {parameters.Dataset}\\images\\test --device {parameters.Device}";

        if (parameters.Augment)
        {
            p.StartInfo.Arguments += " --augment";
        }
        if (parameters.Half)
        {
            p.StartInfo.Arguments += " --half";
        }

        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
        p.OutputDataReceived += new DataReceivedEventHandler(TipPopupHandler);
        p.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
        p.ErrorDataReceived += new DataReceivedEventHandler(TipPopupHandler);
        p.EnableRaisingEvents = true;
        p.Exited += new EventHandler(ProcessExited);
        p.Start();//启动进程
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        startTime = p.StartTime;
    }

    private void OutputHandler(object sender, DataReceivedEventArgs e)
    {
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            outputText.Text += e.Data + Environment.NewLine;
            if (autoScroll.IsChecked == true)
            {
                scrollViewer.UpdateLayout();
                scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
                //scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
            }
        });
    }

    private void TipPopupHandler(object sender, DataReceivedEventArgs e)
    {
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            if (tipPoped == false)
            {
                tipPoped = true;
                tip.IsOpen = true;
                p.OutputDataReceived -= TipPopupHandler;
                p.ErrorDataReceived -= TipPopupHandler;
            }
        });
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

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        MainWindow.MyDispatcherQueue.TryEnqueue(() =>
        {
            //timerText.Text = e.SignalTime.ToString();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = _stopwatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            timerText.Text = startTime + @"    |    ----/--/-- --:--:--    |    " + elapsedTime;
            bottomTimerText.Text = elapsedTime;
        });
    }

    private void StopDetect_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        p.Kill();
    }

    private void OpenOutputFolder(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        if (Directory.Exists(run.Text))
        {
            Process.Start("explorer.exe", run.Text);
        }
        else
        {
            MainWindow.MyDispatcherQueue.TryEnqueue(() =>
            {
                ContentDialog pleaseWait = new()
                {
                    Title = "提示",
                    Content = "文件夹尚未创建完成，请稍后…",
                    CloseButtonText = "关闭",
                    XamlRoot = XamlRoot
                };
                _ = pleaseWait.ShowAsync();
            });
        }
    }

    private void copyButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        DataPackage dataPackage = new DataPackage();
        dataPackage.RequestedOperation = DataPackageOperation.Copy;
        dataPackage.SetText(outputText.Text);
        Clipboard.SetContent(dataPackage);
    }
}
