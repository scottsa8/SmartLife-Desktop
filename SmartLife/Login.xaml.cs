using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using H.NotifyIcon;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SmartLife.views;
using Syncfusion.UI.Xaml.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartLife;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Login : Page
{
    private static String uAttempt = "";
    private static String pAttempt = "";
    private DispatcherTimer dispatcherTimer;

    public Login()
    {
        this.InitializeComponent();
    }
    private async void log(object sender, RoutedEventArgs e)
    {
        uAttempt = user.Text;
        pAttempt = pass.Password;
        int state = await App.getApi().GetToken(uAttempt, pAttempt);
        switch (state)
        {
            //to many attempts
            case 0:
                infobar.Visibility = Visibility.Visible;
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Interval = TimeSpan.FromSeconds(10);
                dispatcherTimer.Start();
                break;
            //wrong info
            case -1:
                credsbck.Visibility = Visibility.Visible;
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Interval = TimeSpan.FromSeconds(10);
                dispatcherTimer.Start();
                break;

            default:
                App.getApi().Discover();
                App.GetFrame().Navigate(typeof(HomePage));
                App.RunUpdater();
                break;
        }

    }
    private void DispatcherTimer_Tick(object? sender, object e)
    {
        infobar.Visibility = Visibility.Collapsed;
        credsbck.Visibility = Visibility.Collapsed;
        if (dispatcherTimer != null) { dispatcherTimer.Stop(); }
        dispatcherTimer = null!;
    }

    private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        if (revealModeCheckBox.IsChecked == true)
        {
            pass.PasswordRevealMode = PasswordRevealMode.Visible;
            revealModeCheckBox.Content = "Hide";
        }
        else
        {
            pass.PasswordRevealMode = PasswordRevealMode.Hidden;
            revealModeCheckBox.Content = "Show";
        }
    }
    private void help(object sender, RoutedEventArgs e)
    {
        
        App.GetFrame().Navigate(typeof(About));
        Button? close = App.GetFrame().FindChild("closeB") as Button;
        if (close != null) { close.Visibility = Visibility.Visible;}

        App.MainWindow!.AppWindow.Resize(new Windows.Graphics.SizeInt32(1600, 900));
        

    }
}
