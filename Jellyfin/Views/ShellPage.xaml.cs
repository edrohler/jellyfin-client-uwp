using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Models;
using Windows.UI.Xaml.Media.Imaging;
using System.Linq;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Helpers;

namespace Jellyfin.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            InitializeComponent();

            // We want to set the app-wide Shell reference right away
            App.Current.Shell = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.PageReadyAsync();


            // Profile Image
            BitmapImage profileImage = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Users/{App.Current.AppUser.Id}/Images/Primary?tag={App.Current.AppUser.PrimaryImageTag}"));
            ProfileImage.Source = profileImage;
            ProfileImage.Width = 40;
            ProfileImage.Height = 40;

            // Profile Name
            AccountNavViewItem.Content = App.Current.AppUser.Name;
            
            // Setting the initial page
            ContentFrame.Navigate(typeof(HomePage));
        }

        // Navigate from outside of the ShellPage
        // i.e. the Home Page Content or Library Page
        public void ChangeMenuSelection(Guid Id)
        {
            MenuDataItem menuItem = ViewModel.MenuItems.Where(i => i.Id == Id).FirstOrDefault();

            if (menuItem == null)
            {
                // Deselect the Menu Item
                // because we're not on a menu item
                NavView.SelectedItem = null;
                // Navigate to the Item ID
                ContentFrame.Navigate(typeof(ItemPage), Id);
            }
            else
            {
                // Navigate to the Menu Item
                NavView.SelectedItem = menuItem;
            }
        }

        private void NavView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            MenuDataItem selectedItem = args.SelectedItem as MenuDataItem;

            if (selectedItem != null)
            {
                switch (selectedItem.Name)
                {
                    case "Home":
                        // Navigate to the Home Page
                        ContentFrame.Navigate(typeof(HomePage));
                        break;
                    default:
                        // Naviagte to the Grid Content Page and Pass Library Id
                        ContentFrame.Navigate(typeof(LibraryPage), selectedItem.Id);
                        break;
                }
            }
        }

        private void NavView_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavViewHelper.NavViewChecker(sender);
        }

        //
        // Will Prompt Logout
        private async void AttemptLogoutAsync()
        {
            MessageDialog md = new MessageDialog("Are you sure you want to logout?", "Logout");

            // Button 1
            md.Commands.Add(new UICommand("Logout", (command) =>
            {
                App.Current.RootFrame.Navigate(typeof(LoginPage));
            }));

            // Button 2. Cancel
            md.Commands.Add(new UICommand("Cancel"));

            // Make sure the default button is the 3rd button (Cancel)
            md.DefaultCommandIndex = 2;

            await md.ShowAsync();
        }

        // Catch if Logout Tapped or Clicked
        private void LogoutNavViewItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            AttemptLogoutAsync();
        }

        // Catch if Logout selected with Enter key
        private void LogoutNavViewItem_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AttemptLogoutAsync();
            }
        }

        // Catch if Profile Tapped or Clicked
        private void AccountNavViewItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            NavView.SelectedItem = null;
            ContentFrame.Navigate(typeof(ProfilePage));
        }

        // Catch if Logout selected with Enter key
        private void AccountNavViewItem_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                NavView.SelectedItem = null;
                ContentFrame.Navigate(typeof(ProfilePage));
            }
        }

        private void SuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Navigate to specific page based on selection chosen.
        }
    }
}
