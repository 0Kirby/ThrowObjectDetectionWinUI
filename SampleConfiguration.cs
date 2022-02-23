// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace ThrowObjectDetection
{
    internal static class Settings
    {
        public const string FeatureName = "高空抛物目标检测";
        public static ElementTheme CurrentTheme = ElementTheme.Default;
    }

    public partial class MainPage : Page
    {
        private readonly List<Scenario> scenarios = new()
        {
            new Scenario() { Title = "主页", Icon = "\uF1AD", ClassName = typeof(HomePage).FullName },
            new Scenario() { Title = "标注", Icon = "\uE70F", ClassName = typeof(AnnotatePage1).FullName },
            new Scenario() { Title = "标注", Icon = "\uE70F", ClassName = typeof(AnnotatePage2).FullName },
            new Scenario() { Title = "标注", Icon = "\uE70F", ClassName = typeof(AnnotatePage3).FullName },
            new Scenario() { Title = "标注", Icon = "\uE70F", ClassName = typeof(AnnotatePage4).FullName },
            new Scenario() { Title = "训练", Icon = "\uE945", ClassName = typeof(TrainPage1).FullName },
            new Scenario() { Title = "训练", Icon = "\uE945", ClassName = typeof(TrainPage2).FullName },
            new Scenario() { Title = "检测", Icon = "\uE8A3", ClassName = typeof(DetectPage1).FullName },
            new Scenario() { Title = "检测", Icon = "\uE8A3", ClassName = typeof(DetectPage2).FullName }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string ClassName { get; set; }
    }
}
