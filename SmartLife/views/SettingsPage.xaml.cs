using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace SmartLife.views;

public sealed partial class SettingsPage : Page
{
    public static ObservableCollection<SettingsOptions> settings = new ObservableCollection<SettingsOptions>();
    public SettingsPage()
    {
        this.InitializeComponent();
    }
    public void Item_Click(object sender, RoutedEventArgs e)
    {
        String tag="";
        try
        {
            Button b = (Button)sender;
            tag=b.Tag as String;
        }
        catch (Exception) { }
        switch (tag)
        {
            case "General":
                HomePage.GetContentFrame().Navigate(
                  typeof(SmartLife.views.General),
                  null,
                  new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                  );
                break;
            case "Account":
                HomePage.GetContentFrame().Navigate(
                     typeof(SmartLife.views.Account),
                     null,
                     new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                     );
                break;
            case "About":
                HomePage.GetContentFrame().Navigate(
                     typeof(SmartLife.views.About),
                     null,
                     new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo()
                     );
                break;
            default:
                break;
        }
    }
    public static void setup()
    {
        settings.Add(new SettingsOptions("General", "\uE7F4"));
        settings.Add(new SettingsOptions("Account", "\uE77B"));
        settings.Add(new SettingsOptions("About", "\uE897"));
    }

}
