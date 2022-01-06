using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ThrowObjectDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnnotatePage2 : Page
    {
        public AnnotatePage2()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        private void HomeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.Current.NavView_SubPage_Navigate("ThrowObjectDetection.AnnotatePage1", new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }
        private void LeftButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.Current.NavView_BackRequested(null, null);
        }
        private void RightButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.Current.NavView_SubPage_Navigate("ThrowObjectDetection.AnnotatePage3", new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }
        private void OpenButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            Process p = new Process();
            string interpreterPath = (string)localSettings.Values["pythonInterpreter"];
            int lastIndex = interpreterPath.LastIndexOf("python");
            string substr = interpreterPath.Substring(0, lastIndex);
            p.StartInfo.FileName = substr + "labelimg"; //虚拟环境中python的安装路径
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();//启动进程
        }
    }
}
