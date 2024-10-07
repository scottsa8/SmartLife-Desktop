using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.AccessControl;
using System.Security.Principal;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartLife.views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class General : Page
{
    private bool pop;
    private bool fav;
    private bool invert;
    public General()
    {
        this.InitializeComponent();
        fav = App.GetFav();
        pop = !App.GetFav();
        if (fav == false) //if pop is on
        {
            tsFav.IsOn = false;
            favouritepanel.IsEnabled = false;
            stkpan.Opacity = 0.5;
            stkpan2.Opacity = 1.0;
            invert = true;
        }
        else // if fav is on
        {
            tsPop.IsOn = false;
            favouritepanel.IsEnabled = true;
            stkpan2.Opacity = 0.5;
            stkpan.Opacity = 1.0;
            invert = false;
        }
        if (App.GetStart())
        {
            bckgrd.Opacity = 1.0;
            bckgrd.IsEnabled = true;
        }
        else
        {
            bckgrd.Opacity = 0.5;
            bckgrd.IsEnabled = false;
        }
    }
    public void SmallPop(object sender, RoutedEventArgs e)
    {
        if (stkpan2 != null)
        {
            if (tsPop.IsOn)
            {
                tsFav.IsEnabled = false;
                tsFav.IsOn = false;
                favouritepanel.IsEnabled = false;
            }
            else
            {
                tsFav.IsEnabled = true;
                tsFav.IsOn = true;
                favouritepanel.IsEnabled = true;
            }
            pop = !pop;
            DisablePop();
            App.UpdateFav(true);
        }
    }
    public void DisablePop()
    {
        bool temp;
        if (invert) { temp = !pop; } else { temp = pop; }
        if (temp)
        {
            stkpan2.Opacity = 1.0;
        }
        else { stkpan2.Opacity = 0.5; }
    }
    public void DisableFav()
    {
        bool temp;
        if (invert) { temp = !fav; } else { temp = fav; }
        if (temp)
        {
            stkpan.Opacity = 1.0;
        }
        else { stkpan.Opacity = 0.5; }
    }
    public void UpdateFav(object sender, RoutedEventArgs e)
    {
        if (favouritepanel != null)
        {
            if (tsFav.IsOn)
            {
                tsPop.IsEnabled = false;
                tsPop.IsOn = false;
                favouritepanel.IsEnabled = true;
            }
            else
            {
                tsPop.IsEnabled = true;
                tsPop.IsOn = true;
                favouritepanel.IsEnabled = false;
            }
            fav = !fav;
            DisableFav();
            App.UpdateFav(false);

        }
    }
    public int GetSelected()
    {
        for (int i = 0; i < App.GetDevices().Count; i++)
        {
            if (App.GetDevices()[i].favourite)
            {
                return i;
            }
        }
        return 0;
    }
    public int GetScale()
    {
        switch (App.Width)
        {
            case 1600:
                return 9;
            case 1280:
                return 8;
            case 1024:
                return 8;

        }
        return 1;
    }
    public void RadioChecked(object sender, RoutedEventArgs e)
    {
        try
        {
            RadioButton r = (RadioButton)sender;
            string t = r.Tag.ToString();
            for (int i = 0; i < App.GetDevices().Count; i++)
            {
                if (App.GetDevices()[i].id.Equals(t))
                {
                    App.GetDevices()[i].favourite = true;
                }
                else
                {
                    App.GetDevices()[i].favourite = false;
                }
            }
        }
        catch (Exception) { }
    }
    public void ChangeWindow(object sender, RoutedEventArgs e)
    {
        MenuFlyoutItem b = (MenuFlyoutItem)sender;
        switch (b.Tag)
        {
            case "1":
                App.MainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1024, 576));
                App.UpdateSettings(new Windows.Graphics.SizeInt32(1024, 576));
                drop.Content = "1024 x 576";
                break;
            case "2":
                App.MainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1280, 720));
                App.UpdateSettings(new Windows.Graphics.SizeInt32(1280, 720));
                drop.Content = "1280 x 720";
                break;
            case "3":
                App.MainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1600, 900));
                App.UpdateSettings(new Windows.Graphics.SizeInt32(1600, 900));
                drop.Content = "1600 x 900";
                break;
            default:
                break;
        }
    }
    public void StartUpOn(object sender, RoutedEventArgs e) 
    {
        App.ToggleStart(true);
        bckgrd.Opacity = 1;
        bckgrd.IsEnabled = true;

    
        RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
        RegistryKey reg = key.OpenSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)!;
        reg.SetValue("SmartLife", AppDomain.CurrentDomain.BaseDirectory + "SmartLife.exe", RegistryValueKind.String);

        for (int i = 0; i < reg.GetValueNames().Length; i++)
        {
            System.Diagnostics.Debug.WriteLine(reg.GetValueNames()[i]);
            System.Diagnostics.Debug.WriteLine(reg.GetValue(reg.GetValueNames()[i]));
        }
        key.Close();
        reg.Close();
    }
    public void StartUpOff(object sender, RoutedEventArgs e) 
    {
        App.ToggleStart(false);
        bckgrd.Opacity = 0.5;
        bckgrd.IsEnabled = false;
    }
    public void BackOn(object sender, RoutedEventArgs e)
    {
        App.ToggleStartbck(true);
    }
    public void BackOff(object sender, RoutedEventArgs e)
    {
        App.ToggleStartbck(false);
    }

}
