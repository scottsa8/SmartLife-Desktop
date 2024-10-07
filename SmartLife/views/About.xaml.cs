using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartLife.views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class About : Page
{
    public About()
    {
        this.InitializeComponent();
    }
    private async void Github(object sender, RoutedEventArgs e)
    {
       await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/scottsa8/SmartLife-Desktop")); 
    }
    private async void Store(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("http://google.com")); //need Windows URL
    }
    private async void MyGithub(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/scottsa8")); 
    }
    private async void Discord(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discord.gg/bJv5ktW9pb")); 
    }
    private async void Twitter(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://x.com/veteranfall3")); 
    }
    private async void GithubSup(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/scottsa8/SmartLife-Desktop/issues"));
    }
    private async void Smart(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://smart-life-app.com"));
    }
    private async void GooglePlay (object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://play.google.com/store/apps/details?id=com.tuya.smartlife"));
    }
    private async void AppStore(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://apps.apple.com/us/app/smart-life-smart-living/id1115101477"));
    }
    private void Close(object sender, RoutedEventArgs e) 
    {
        App.GetFrame().Navigate(typeof(Login));
    }
}
