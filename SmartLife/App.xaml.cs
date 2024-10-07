using SmartLife;
using SmartLife.views;
using Microsoft.UI.Windowing;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Timers;
using Windows.Graphics;
using System.Runtime.Versioning;
using H.NotifyIcon;
using Uno.Extensions.Toolkit;
using System;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.Win32.TaskScheduler;
using Microsoft.UI;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.AccessControl;




namespace SmartLife;
public partial class App : Application
{
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXlcdXRWQmFfWUZxXUY=");
        this.InitializeComponent();
    }

    public static Window? MainWindow { get; set; }
    protected IHost? Host { get; private set; }
    public static bool HandleClosedEvents { get; set; } = true;
    private static readonly Frame rootFrame = new Frame();
    private static ObservableCollection<Device> devices = new ObservableCollection<Device>();
    private static ObservableCollection<Notification> noti = new ObservableCollection<Notification>();
    private static readonly ApiCaller api = new ApiCaller();
    private static User? user;
    private static bool logged = false;
    private static SizeInt32 WindowSize = new SizeInt32(1600, 900);
    public static int Width = 1600;
    public static int Height = 900;
    public static ObservableCollection<int> TotalNotifications = new ObservableCollection<int>();
    private static bool FavEnabled = true;
    private static bool Startup = false;
    private static bool Startupbck = false;
    private static readonly string path = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\Settings.ini"));

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
        #region unoshit
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Warning);
                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                )
                .UseLocalization()
                .UseThemeSwitching( )
            );
         MainWindow = builder.Window;

#if DEBUG
        MainWindow.EnableHotReload();
#endif
        #endregion        
        Host = builder.Build();

        MainWindow = new Window();
        
        MainWindow.Content = rootFrame;
        user = new User();
        ReadSettings();
        user.login();
        if (logged)
        {
            api.Refresh();
            api.Discover();
            rootFrame.Navigate(typeof(HomePage));
            RunUpdater();
        }
        else
        {
            rootFrame.Navigate(typeof(Login));
        }

        //disable resizing
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow);
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        if (appWindow.Presenter is OverlappedPresenter p)
        {
            p.IsResizable = false;
            p.IsMaximizable = false;
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

        Windows.Graphics.SizeInt32 size;
        try
        {
            size = new Windows.Graphics.SizeInt32(Width, Height);
        }
        catch (Exception)
        {
            size = new Windows.Graphics.SizeInt32(1600, 900);
        }
        MainWindow.AppWindow.Resize(size);
        MainWindow.AppWindow.Title = "Smart Life (unofficial)";
        MainWindow.AppWindow.SetIcon("Assets/Icons/smart-life.ico");


        SetBackDrop(0, MainWindow);
        if (!Startupbck) { MainWindow.Activate(); }
        
     
       
        MainWindow.Closed += (sender, args) =>
        {
            if (!logged)
            {
                Environment.Exit(0);
            }
            else
            {
                if (HandleClosedEvents)
                {
                    args.Handled = true;
                    MainWindow.AppWindow.Hide();
                }
            }
        };
        SettingsPage.setup();

    }
    public static void RunUpdater()
    {
        Thread refresher = new Thread(Refresh_Access_Token);
        refresher.Start();
    }
    public static Frame GetFrame()
    {
        return rootFrame;
    }
    public static User GetUser()
    {
        return user!;
    }
    public static bool GetFav()
    {
        return FavEnabled;
    }
    public static bool GetPop()
    {
        return !FavEnabled;
    }
    public static bool GetStart()
    {
        return Startup;
    }
    public static bool GetStartbck()
    {
        return Startupbck;
    }
    public static void UpdateFav(bool update)
    {
        FavEnabled = update;
        UpdateSettings(new SizeInt32(-1,-1));
    }
    public static void ToggleStart(bool start)
    {
        Startup = start;
        //task scheduler
        UpdateSettings(new SizeInt32(-1, -1));
    }
    public static void ToggleStartbck(bool state)
    {
        Startupbck = state;
        UpdateSettings(new SizeInt32(-1, -1));
    }
    public static ApiCaller getApi()
    {
        return api;
    }
    public static ObservableCollection<Device> GetDevices()
    {
        return devices;
    }
    public static ObservableCollection<Notification> GetNotis()
    {
        return noti;
    }  
    public static int GetTotal() { return TotalNotifications.Count; }
    public static void UpdateNotification(bool rem, string n, string i, Brush s, string ic)
    {
        if (rem) 
        {
            if (n.Equals("All"))
            {
                noti.Clear();
                TotalNotifications.Clear();
                return;
            }
            try
            {
                var index = noti.Select((item, index) => new { Item = item, Index = index }).First(i => i.Item.Name.Equals(n)).Index;
                noti.RemoveAt(index);
                TotalNotifications.RemoveAt(TotalNotifications.Count - 1);
            }
            catch (Exception)
            { //no warning
            }
        }
        else
        {
            for (int x = 0; x < noti.Count; x++)
            {
                if (noti[x].Time.Equals(DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")) && noti[x].Name.Equals(n))
                {
                    return;
                }
            }
            Random rnd = new Random();
            int temp = rnd.Next(100);
            TotalNotifications.Add(temp);
            noti.Add(new Notification(n, i, s, ic,DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")));
        }
    }
    public static void setLogin(bool b)
    {
        logged = b;
    }
    public static void Refresh_Access_Token()
    {
        System.Timers.Timer timer = new System.Timers.Timer(30000);
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = true;
        timer.Enabled = true;

    }
    private static void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {
        if (logged)
        {
            System.Diagnostics.Debug.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                           e.SignalTime);
            api.Refresh();
        }
    }
    private static void ReadSettings()
    {
        try
        {
            StreamReader sr = new StreamReader(path);
            string? line = sr.ReadLine();
            if (line != null)
            {
                while (line != null)
                {
                    if (line.StartsWith("Height:"))
                    {
                        Height = int.Parse(line.Split(":")[1]);

                    }
                    else if (line.StartsWith("Width:"))
                    {
                        Width = int.Parse(line.Split(":")[1]);
                    }
                    else if (line.StartsWith("SaveState:"))
                    {
                        if (line.Split(":")[1].Equals("True"))
                        {
                            user!.SetSaveState(true);
                        }
                        else
                        {
                            user!.SetSaveState(false);
                        }
                    }
                    else if (line.StartsWith("Fav:"))
                    {
                        if (line.Split(":")[1].Equals("True"))
                        {
                            FavEnabled = true;
                        }
                        else
                        {
                            FavEnabled = false;
                        }
                    }
                    else if (line.StartsWith("Start:"))
                    {
                        if (line.Split(":")[1].Equals("True"))
                        {
                            Startup = true;
                        }
                        else
                        {
                            Startup = false;
                        }
                    }
                    else if (line.StartsWith("Startbck:"))
                    {
                        if (line.Split(":")[1].Equals("True"))
                        {
                            Startupbck = true;
                        }
                        else
                        {
                            Startupbck = false;
                        }
                    }
                    line = sr.ReadLine();
                }
            }
            sr.Close();
        }catch(IOException) 
        {
            StreamWriter sw = File.AppendText(path);
            sw.WriteLine("Height:" + Height);
            sw.WriteLine("Width:" + Width);
            sw.WriteLine("SaveState:" + user!.GetSaveState());
            sw.WriteLine("Fav:" + FavEnabled);
            sw.WriteLine("Start:" + Startup);
            sw.WriteLine("Startbck:" + Startupbck);
            sw.Close();
            File.SetAttributes(path, System.IO.FileAttributes.Hidden);
        }
    }
    public static void UpdateSettings(Windows.Graphics.SizeInt32 s)
    {
        try
        {
            if (s.Width == -1 && s.Height == -1) { s.Width = Width; s.Height = Height; }else { Width= s.Width; Height= s.Height; }
            File.Delete(path);
            StreamWriter sw = File.AppendText(path);
            sw.WriteLine("Height:" + s.Height);
            sw.WriteLine("Width:" + s.Width);
            sw.WriteLine("SaveState:" + user!.GetSaveState());
            sw.WriteLine("Fav:" + FavEnabled);
            sw.WriteLine("Start:" + Startup);
            sw.WriteLine("Startbck:" + Startupbck);
            sw.Close();
            File.SetAttributes(path, System.IO.FileAttributes.Hidden);
        }catch(IOException) { }
    }
    public static String GetSizeText()
    {
        return MainWindow!.AppWindow.Size.Width.ToString() + " x " + MainWindow.AppWindow.Size.Height.ToString();
    }
    public static bool SetBackDrop(int type,Window window)
    {
        if(type == 0)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
            {
                Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop DesktopAcrylicBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
                window!.SystemBackdrop = DesktopAcrylicBackdrop;

                return true; // Succeeded.
            }

            return false; // DesktopAcrylic is not supported on this system.
        }
        else if (type == 1)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                Microsoft.UI.Xaml.Media.MicaBackdrop DesktopAcrylicBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
                window!.SystemBackdrop = DesktopAcrylicBackdrop;

                return true; // Succeeded.
            }

            return false; // DesktopAcrylic is not supported on this system.
        }
        else
        {
            return false;
        }
       
    }
    public static bool GetSaveState()
    {
        return user!.GetSaveState();
    }
}


/** 
 * COSMETIC
 * ------------
 * THEMEING - PICK A THEME
 * OVERALL UI AND ANIMATIONS
 * format popup tray to look abit nicer
 * notifications -> flash when same noti?
 * 
 * FUNCTIONAL
 * ------------
 * MAYBE ADD NEW TASK BAR ICON -> OPTIONAL?
 * CLEAN THIS SHIT UP 
 * Start app on boot - auto start in background? - half done ?
 * **/
