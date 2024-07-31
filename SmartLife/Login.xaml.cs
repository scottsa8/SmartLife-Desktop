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
    private static Window w;

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
                if (w != null)
                {
                    w.Hide();
                    w.Close();
                    w = null!;
                }
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
        if (w == null)
        {
            w = new Window();
            Frame f = new Frame();
            f.Navigate(typeof(About));
            w.Content = f;

            //disable resizing
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(w);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            if (appWindow.Presenter is OverlappedPresenter p)
            {
                p.IsResizable = false;
                p.IsMinimizable = false;
                p.IsMaximizable = false;
                p.IsAlwaysOnTop = true;
            }
            //move to center
            Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var CenteredPosition = appWindow.Position;
                CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                appWindow.Move(CenteredPosition);
                appWindow.SetIcon("Assets/Icons/smart-life.ico");
            }
            w.AppWindow.Resize(new Windows.Graphics.SizeInt32(1600, 900));
            w.ExtendsContentIntoTitleBar = true;
            f.Padding =  new Thickness(0, 20, 0,0);
            App.SetBackDrop(0, w);
            w.Activate();

            w.Closed += (sender, args) =>
            {
                w.Hide();
                w.Close();
                w = null!;
            };
        }
        else
        {
            w.Hide();
            w.Close();
            w = null!;
        }

    }
}
