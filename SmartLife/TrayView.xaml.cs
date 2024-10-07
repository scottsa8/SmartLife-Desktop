
using SmartLife.views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.EfficiencyMode;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Devices.Display.Core;
using Microsoft.UI.Windowing;
using Uno;

namespace SmartLife;

[ObservableObject]
public sealed partial class TrayView : UserControl
{
    [ObservableProperty]
    private bool _isWindowVisible;

    private static Window ?window;
    public TrayView()
    {
        InitializeComponent();
        //Menu.SystemBackdrop = new DesktopAcrylicBackdrop();
        FirstRun();
    }
    public async void FirstRun()
    {
        await FirstRunTask();
    }
    public async Task<bool> FirstRunTask()
    {
        await Task.Delay(1000);
        Menu.Items.Insert(0, new MenuFlyoutSeparator() { Tag = "SEP" });
        for (int i = 0; i < App.GetDevices().Count; i++)
        {
            string txt = "Toggle " + App.GetDevices()[i].name;
            string id = App.GetDevices()[i].name.ToUpper();
            Menu.Items.Insert(i, new MenuFlyoutItem()
            {
                Command = TurnOnOffCommand,
                CommandParameter = i,
                Text = txt,
                Tag = "D_"+id,
                Icon = new ImageIcon() { Source = new BitmapImage(new Uri(App.GetDevices()[i].icon)) }
            });
        }
        return true;
    }
    [RelayCommand]
    public void updateDevices()
    {
        //should be fixed?
        App.getApi().Discover();
        for (int c = 0; c < Menu.Items.Count; c++)
        {
            for (int x =0;x< App.GetDevices().Count; x++)
            {
                string txt = "D_" + App.GetDevices()[x].name.ToUpper();
                try
                {
                    var index = Menu.Items.Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.Tag.Equals((txt))).Index;
                    System.Diagnostics.Debug.WriteLine(index +"already added");
                }
                catch (Exception) 
                {
                    System.Diagnostics.Debug.WriteLine("add");
                }
               
            }

        }
    }
    [RelayCommand]
    public void ShowHideWindow()
    {
        //increases memory usage here
        var window = App.MainWindow;
        if (window == null)
        {
            return;
        }

        if (window.Visible)
        {
            window.Hide();
            EfficiencyModeUtilities.SetEfficiencyMode(true);                
        }
        else
        {
            if (HomePage.GetContentFrame().CurrentSourcePageType != typeof(DisplayView))
            {
                HomePage.GetContentFrame().Navigate(
                 typeof(SmartLife.views.DeviceView),
                 null,
                 new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                 );
            }
            window.Show();
            EfficiencyModeUtilities.SetEfficiencyMode(false);
          
        }
        IsWindowVisible = window.Visible;
        
    }

    [RelayCommand]
    public void OpenSettings()
    {
        System.Diagnostics.Debug.WriteLine("clicked");
        var window = App.MainWindow;
        if (window == null)
        {
            return;
        }
        if (window.Visible) { 
        
        }
        else
        {
            window.Show();
        }
        HomePage.GetContentFrame().Navigate(
                  typeof(SmartLife.views.SettingsPage),
                  null,
                  new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                  );
        IsWindowVisible = window.Visible;
    }
    [RelayCommand]
    public void ExitApplication()
    {
        App.HandleClosedEvents = false;
        TrayIcon.Dispose();

        App.MainWindow?.Close();
    }
    [RelayCommand]
   public void TurnOnOff(int sender)
    {
        App.getApi().TurnOnOff(sender, false);
    }
    [RelayCommand]
    public void ToggleFavourite()
    {
        if (App.GetFav())
        {
            for (int i = 0; i < App.GetDevices().Count; i++)
            {
                if (App.GetDevices()[i].favourite)
                {
                    System.Diagnostics.Debug.WriteLine("fav:" + i);
                    App.getApi().TurnOnOff(i, false);
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        else
        {
            //OPEN POPUP
            if (window == null)
            {
                if (App.MainWindow!.Visible)
                {
                    HomePage.GetContentFrame().Navigate(
                typeof(SmartLife.HomePage),
                null,
                new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                );
                }
                else
                {
                    window = new Window();
                    Frame t = new Frame();
                    window.Content = t;
                    t.Navigate(typeof(TaskFlyout));
                    window.AppWindow.Resize(new Windows.Graphics.SizeInt32(500, 600));
                    var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                    var appWindow = AppWindow.GetFromWindowId(windowId);
                    if (appWindow.Presenter is OverlappedPresenter p)
                    {
                        p.IsResizable = false;
                        p.IsAlwaysOnTop = true;
                        p.IsMaximizable = false;
                        p.IsMinimizable = false;
                    }
                    //move to center
                    Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                    if (displayArea is not null)
                    {
                        var CenteredPosition = appWindow.Position;
                        CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width));
                        CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) - 200);

                        appWindow.Move(CenteredPosition);
                        appWindow.SetIcon("Assets/Icons/smart-life.ico");
                    }
                    window.ExtendsContentIntoTitleBar = true;
                    App.SetBackDrop(0, window);
                    window.Activate();

                    window.Closed += (sender, args) =>
                    {
                        System.Diagnostics.Debug.WriteLine("closed");
                        window.Close();
                    };
                }
            }
            else
            {
                window.Hide();
                window.Close();
                window = null!;
            }
        }
    }
    public static void FlyoutSettings(int where)
    {
        if (window != null)
        {
            
            window.Hide();
            window.Close();
            window = null!;
        }
        if (where == 0)
        {
            if (App.MainWindow!.Visible == false)
            {
                App.MainWindow.Show();
            }
            HomePage.GetContentFrame().Navigate(
                      typeof(SmartLife.views.SettingsPage),
                      null,
                      new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                      );
        }
        else if (where == 1)
        {
            if (App.MainWindow!.Visible == false)
            {
                App.MainWindow.Show();
            }
            HomePage.GetContentFrame().Navigate(
                   typeof(SmartLife.HomePage),
                   null,
                   new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                   );
        }
        else 
        {
            if (window != null)
            {
                window.Hide();
                window.Close();
                window = null!;
            }
        }
    }
}
