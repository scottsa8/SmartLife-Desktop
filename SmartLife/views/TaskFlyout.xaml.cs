using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartLife.views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TaskFlyout : Page
{
    public TaskFlyout()
    {
        this.InitializeComponent();
    }
    private void TurnOnOff(object sender, RoutedEventArgs e)
    {
        App.getApi().TurnOnOff(sender, false);
        TrayView.FlyoutSettings(-1);
    }
    private void Refresh(object sender, RoutedEventArgs e)
    {
        App.getApi().Discover();
    }
    private void brightness(object sender, RoutedEventArgs e)
    {
        App.getApi().Brightness(sender, false);
        TrayView.FlyoutSettings(-1);
    }
    private void Color(object sender, Syncfusion.UI.Xaml.Editors.SelectedBrushChangedEventArgs e)
    {
        App.getApi().ChangeColor(sender, false, e);
        TrayView.FlyoutSettings(-1);
    }
    private void Settings(object sender, RoutedEventArgs e)
    {
        TrayView.FlyoutSettings(0);
    }
    private void Home(object sender, RoutedEventArgs e)
    {
        TrayView.FlyoutSettings(1);
    }
    private void Close(object sender, RoutedEventArgs e)
    {
        TrayView.FlyoutSettings(-1);
    }
}
