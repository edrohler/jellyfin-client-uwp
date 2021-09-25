﻿using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Models;

namespace Jellyfin.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            this.InitializeComponent();

            // We want to set the app-wide Shell reference right away
            App.Current.Shell = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.PageReadyAsync();

            // Setting the initial page
            this.ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // This is how you'd navigate when the selection changes in the NavigationView
            // Will need to be more dynamic. Use a single content collection page that queries the collection on load.
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                switch (selectedItem.Content)
                {
                    case "Movies":
                        //ContentFrame.Navigate(typeof(MoviesPage));
                        break;
                    case "Music":
                        //ContentFrame.Navigate(typeof(MusicPage));
                        break;
                    case "Photos":
                        //ContentFrame.Navigate(typeof(PhotosPage));
                        break;
                    case "Games":
                        //ContentFrame.Navigate(typeof(GamesPage));
                        break;
                    default:
                        ContentFrame.Navigate(typeof(HomePage));
                        break;
                }
            }
        }


        //
        // Since the NavigationView doesn't have the same features in the older versions of Windows 10.
        // This handy checker lets you use each version of the NavigationView's features with confidence
        // The values set below are recommended by Microsoft.
        private void NavView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var navView = sender as NavigationView;
            var rootGrid = VisualTreeHelper.GetChild(navView, 0) as Grid;

            // SDK 18362 (1903)
            // SDK 17763 (1809)
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                // Find the back button.
                var paneToggleButtonGrid = VisualTreeHelper.GetChild(rootGrid, 0) as Grid;
                var buttonHolderGrid = VisualTreeHelper.GetChild(paneToggleButtonGrid, 1) as Grid;
                var navigationViewBackButton = VisualTreeHelper.GetChild(buttonHolderGrid, 0) as Button;

                navigationViewBackButton.AccessKey = "A";

                if (navView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
                {
                    // Set back button key tip placement mode.
                    navigationViewBackButton.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Bottom;

                    // Find the settings item and set properties.
                    var grid = VisualTreeHelper.GetChild(rootGrid, 1) as Grid;
                    var topNavArea = VisualTreeHelper.GetChild(grid, 0) as StackPanel;
                    var topNavGrid = VisualTreeHelper.GetChild(topNavArea, 1) as Grid;
                    var settingsTopNavPaneItem = VisualTreeHelper.GetChild(topNavGrid, 7) as NavigationViewItem;

                    settingsTopNavPaneItem.AccessKey = "S";
                    settingsTopNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Bottom;
                }
                else
                {
                    // Set back button key tip placement mode.
                    navigationViewBackButton.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;

                    // Find the settings item and set properties.
                    var grid = VisualTreeHelper.GetChild(rootGrid, 1) as Grid;
                    var rootSplitView = VisualTreeHelper.GetChild(grid, 1) as SplitView;
                    var grid2 = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
                    var paneRoot = VisualTreeHelper.GetChild(grid2, 0) as Grid;
                    var border = VisualTreeHelper.GetChild(paneRoot, 0) as Border;
                    var paneContentGrid = VisualTreeHelper.GetChild(border, 0) as Grid;
                    var settingsNavPaneItem = VisualTreeHelper.GetChild(paneContentGrid, 6) as NavigationViewItem;

                    settingsNavPaneItem.AccessKey = "S";
                    settingsNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;
                }
            }
            // SDK 17134 (1803)
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                // Find the back button and set properties.
                var paneToggleButtonGrid = VisualTreeHelper.GetChild(rootGrid, 0) as Grid;
                var buttonHolderGrid = VisualTreeHelper.GetChild(paneToggleButtonGrid, 1) as Grid;
                var navigationViewBackButton = VisualTreeHelper.GetChild(buttonHolderGrid, 0) as Button;

                navigationViewBackButton.AccessKey = "A";
                navigationViewBackButton.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;

                // Find the settings item and set properties.
                var rootSplitView = VisualTreeHelper.GetChild(rootGrid, 1) as SplitView;
                var grid = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
                var paneRoot = VisualTreeHelper.GetChild(grid, 0) as Grid;
                var border = VisualTreeHelper.GetChild(paneRoot, 0) as Border;
                var paneContentGrid = VisualTreeHelper.GetChild(border, 0) as Grid;
                var settingsNavPaneItem = VisualTreeHelper.GetChild(paneContentGrid, 5) as NavigationViewItem;

                settingsNavPaneItem.AccessKey = "S";
                settingsNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;
            }
            // SDK 16299 (Fall Creator's Update)
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
            {
                // Find the settings item and set properties.
                var rootSplitView = VisualTreeHelper.GetChild(rootGrid, 1) as SplitView;
                var grid = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
                var paneRoot = VisualTreeHelper.GetChild(grid, 0) as Grid;
                var border = VisualTreeHelper.GetChild(paneRoot, 0) as Border;
                var paneContentGrid = VisualTreeHelper.GetChild(border, 0) as Grid;
                var settingsNavPaneItem = VisualTreeHelper.GetChild(paneContentGrid, 4) as NavigationViewItem;

                settingsNavPaneItem.AccessKey = "S";
                settingsNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;
            }
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

        // Catch if Logout Selected with Enter Key
        private void LogoutNavViewItem_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AttemptLogoutAsync();
            }
        }
    }
}
