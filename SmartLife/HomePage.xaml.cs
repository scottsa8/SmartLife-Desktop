using SmartLife.views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace SmartLife;

public sealed partial class HomePage : Page
{
    private static Frame frame;
 
    public HomePage()
    {
        this.InitializeComponent();
        NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().First();
        ContentFrame.Navigate(
                   typeof(SmartLife.views.DeviceView),
                   null,
                   new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo()
                   );
        frame = ContentFrame;
        if (!App.GetUser().GetSaveState())
        {
            App.UpdateNotification(false,"Saving disabled","Enable in settings to auto login on app startup!", new SolidColorBrush(Colors.LightGoldenrodYellow), "\uE946");
        }
    }
   public static Frame GetContentFrame()
    {
        return frame;
    }
    public string GetAppTitleFromSystem()
    {
        return Windows.ApplicationModel.Package.Current.DisplayName;
    }
    private void NavigationViewControl_ItemInvoked(NavigationView sender,
                  NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked == true)
        {
            ContentFrame.Navigate(typeof(SmartLife.views.SettingsPage), null, args.RecommendedNavigationTransitionInfo);
        } else if (args.InvokedItemContainer.Tag.Equals("REFRESH"))
        {
            App.getApi().Discover();
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.ElementAt(0);        }
        else if (args.InvokedItemContainer != null && (args.InvokedItemContainer.Tag != null))
        {
            Type newPage = Type.GetType(args.InvokedItemContainer.Tag.ToString());
            ContentFrame.Navigate(
                   newPage,
                   null,
                   args.RecommendedNavigationTransitionInfo
                   );
        }
    }

    private void NavigationViewControl_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack) ContentFrame.GoBack();
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        NavigationViewControl.IsBackEnabled = ContentFrame.CanGoBack;

        if (ContentFrame.SourcePageType == typeof(SmartLife.views.SettingsPage))
        {
            // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
            NavigationViewControl.SelectedItem = (NavigationViewItem)NavigationViewControl.SettingsItem;
        }
        else if (ContentFrame.SourcePageType == typeof(SmartLife.HomePage)){App.GetFrame().Navigate(typeof(HomePage)); }
        else if (ContentFrame.SourcePageType == typeof(SmartLife.views.General)) { ContentFrame.Navigate(typeof(General));}
        else if (ContentFrame.SourcePageType == typeof(SmartLife.views.Account)) { ContentFrame.Navigate(typeof(Account)); }
        else if (ContentFrame.SourcePageType == typeof(SmartLife.views.About)) { ContentFrame.Navigate(typeof(About)); }
        else if (ContentFrame.SourcePageType != null)
        {
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems
                .OfType<NavigationViewItem>()
                .First(n => n.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));
        }

        NavigationViewControl.Header = ((NavigationViewItem)NavigationViewControl.SelectedItem)?.Content?.ToString();
    }

}

