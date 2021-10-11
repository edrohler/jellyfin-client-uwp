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

            LibNavView.SelectedItem = ViewModel.LibraryPageMenuItems.FirstOrDefault();
        }

        private void LibNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is MenuDataItem selectedItem)
            {
                switch (selectedItem)
                {
                    default:
                        LibraryContentFrame.Navigate(typeof(ItemsPage), selectedItem.Id);
                        break;
                }
            }
        }
    }
}
