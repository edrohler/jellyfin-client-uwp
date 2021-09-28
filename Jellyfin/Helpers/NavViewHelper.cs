using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Jellyfin.Helpers
{
    public static class NavViewHelper
    {
        //
        // Since the NavigationView doesn't have the same features in the older versions of Windows 10.
        // This handy checker lets you use each version of the NavigationView's features with confidence
        // The values set below are recommended by Microsoft.
        public static void NavViewChecker(object sender)
        {
            NavigationView navView = sender as NavigationView;
            Grid rootGrid = VisualTreeHelper.GetChild(navView, 0) as Grid;

            // SDK 18362 (1903)
            // SDK 17763 (1809)
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                // Find the back button.
                Grid paneToggleButtonGrid = VisualTreeHelper.GetChild(rootGrid, 0) as Grid;
                Grid buttonHolderGrid = VisualTreeHelper.GetChild(paneToggleButtonGrid, 1) as Grid;
                Button navigationViewBackButton = VisualTreeHelper.GetChild(buttonHolderGrid, 0) as Button;

                navigationViewBackButton.AccessKey = "A";

                if (navView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
                {
                    // Set back button key tip placement mode.
                    navigationViewBackButton.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Bottom;

                    // Find the settings item and set properties.
                    Grid grid = VisualTreeHelper.GetChild(rootGrid, 1) as Grid;
                    StackPanel topNavArea = VisualTreeHelper.GetChild(grid, 0) as StackPanel;
                    Grid topNavGrid = VisualTreeHelper.GetChild(topNavArea, 1) as Grid;
                    NavigationViewItem settingsTopNavPaneItem = VisualTreeHelper.GetChild(topNavGrid, 7) as NavigationViewItem;

                    settingsTopNavPaneItem.AccessKey = "S";
                    settingsTopNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Bottom;
                }
                else
                {
                    // Set back button key tip placement mode.
                    navigationViewBackButton.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;

                    // Find the settings item and set properties.
                    Grid grid = VisualTreeHelper.GetChild(rootGrid, 1) as Grid;
                    SplitView rootSplitView = VisualTreeHelper.GetChild(grid, 1) as SplitView;
                    Grid grid2 = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
                    Grid paneRoot = VisualTreeHelper.GetChild(grid2, 0) as Grid;
                    Border border = VisualTreeHelper.GetChild(paneRoot, 0) as Border;
                    Grid paneContentGrid = VisualTreeHelper.GetChild(border, 0) as Grid;
                    NavigationViewItem settingsNavPaneItem = VisualTreeHelper.GetChild(paneContentGrid, 6) as NavigationViewItem;

                    settingsNavPaneItem.AccessKey = "S";
                    settingsNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;
                }
            }
            // SDK 17134 (1803)
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                // Find the back button and set properties.
                Grid paneToggleButtonGrid = VisualTreeHelper.GetChild(rootGrid, 0) as Grid;
                Grid buttonHolderGrid = VisualTreeHelper.GetChild(paneToggleButtonGrid, 1) as Grid;
                Button navigationViewBackButton = VisualTreeHelper.GetChild(buttonHolderGrid, 0) as Button;

                navigationViewBackButton.AccessKey = "A";
                navigationViewBackButton.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;

                // Find the settings item and set properties.
                SplitView rootSplitView = VisualTreeHelper.GetChild(rootGrid, 1) as SplitView;
                Grid grid = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
                Grid paneRoot = VisualTreeHelper.GetChild(grid, 0) as Grid;
                Border border = VisualTreeHelper.GetChild(paneRoot, 0) as Border;
                Grid paneContentGrid = VisualTreeHelper.GetChild(border, 0) as Grid;
                NavigationViewItem settingsNavPaneItem = VisualTreeHelper.GetChild(paneContentGrid, 5) as NavigationViewItem;

                settingsNavPaneItem.AccessKey = "S";
                settingsNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;
            }
            // SDK 16299 (Fall Creator's Update)
            else if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
            {
                // Find the settings item and set properties.
                SplitView rootSplitView = VisualTreeHelper.GetChild(rootGrid, 1) as SplitView;
                Grid grid = VisualTreeHelper.GetChild(rootSplitView, 0) as Grid;
                Grid paneRoot = VisualTreeHelper.GetChild(grid, 0) as Grid;
                Border border = VisualTreeHelper.GetChild(paneRoot, 0) as Border;
                Grid paneContentGrid = VisualTreeHelper.GetChild(border, 0) as Grid;
                NavigationViewItem settingsNavPaneItem = VisualTreeHelper.GetChild(paneContentGrid, 4) as NavigationViewItem;

                settingsNavPaneItem.AccessKey = "S";
                settingsNavPaneItem.KeyTipPlacementMode = Windows.UI.Xaml.Input.KeyTipPlacementMode.Right;
            }
        }
    }
}
