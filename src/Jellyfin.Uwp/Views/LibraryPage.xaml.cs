using Jellyfin.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class LibraryPage : Page
    {
        public LibraryPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.PageReady((Guid)e.Parameter);

            object SelectedItem = ViewModel.LibraryPageMenuItems.FirstOrDefault();

            if (SelectedItem != null)
            {
                LibNavView.SelectedItem = ViewModel.LibraryPageMenuItems.FirstOrDefault();
            } else
            {
                LibraryContentFrame.Navigate(typeof(ItemsPage), (Guid)e.Parameter);
            }
        }

        private void LibNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is MenuDataItem selectedItem)
            {
                switch (selectedItem.Name)
                {
                    case "Suggestions":
                        LibraryContentFrame.Navigate(typeof(SuggestionsPage), ViewModel.BaseItem.Id);
                        break;
                    default:
                        LibraryContentFrame.Navigate(typeof(ItemsPage), selectedItem.Id);
                        break;
                }
            }
        }
    }
}
