using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Uno;
using ABI.Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartLife.views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Account : Page
{
    public Account()
    {
        this.InitializeComponent();
        if (App.GetUser().GetSaveState())
        {
            info.Visibility = Visibility.Collapsed;
        }
     
    }
    public void Logout(object sender, RoutedEventArgs e)
    {
        App.GetUser().StoreCreds("", "");
        App.setLogin(false);
        App.GetFrame().Navigate(typeof(Login));
    }
   
   
   
    public void AllowSave(object sender, RoutedEventArgs e)
    {
        if (App.GetUser().GetSaveState() == false)
        {
            App.GetUser().StoreCreds(App.GetUser().GetAT(),App.GetUser().GetRT());
        }
        App.GetUser().SetSaveState(true);

        info.Visibility = Visibility.Collapsed;
        toggle.IsOn = true;
    }
    private void DontSave(InfoBar sender, object args)
    {
        App.GetUser().DelSave();
        App.GetUser().SetSaveState(false);
    }
    private void SaveButton(object sender, RoutedEventArgs e)
    {
        ToggleSwitch t = (ToggleSwitch)sender;
        if (t.IsOn)
        {
            if(info.Visibility == Visibility.Visible) {info.Visibility = Visibility.Collapsed;}
            if (!progress.IsActive) { progress.IsActive = true; }
            if (progress.Visibility == Visibility.Collapsed) { progress.Visibility = Visibility.Visible; }
            //no delay for animations
                      
            App.GetUser().SetSaveState(true);
            App.UpdateNotification(true, "Saving disabled", "Enable in settings to auto login on app startup!", new SolidColorBrush(Colors.LightGoldenrodYellow), "\uE946");
            App.GetUser().StoreCreds(App.GetUser().GetAT(), App.GetUser().GetRT());
            progress.IsActive = false;
            progress.Visibility = Visibility.Collapsed;
        }
        else {
            App.GetUser().SetSaveState(false);
            App.UpdateNotification(false, "Saving disabled", "Enable in settings to auto login on app startup!", new SolidColorBrush(Colors.LightGoldenrodYellow), "\uE946");
            if (info.Visibility == Visibility.Collapsed) { info.Visibility = Visibility.Visible; }
            App.GetUser().DelSave();
        }
    }   
    public void AddTray(object sender, RoutedEventArgs e)
    {

    }
}
