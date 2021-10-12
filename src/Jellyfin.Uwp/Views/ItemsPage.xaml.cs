using Jellyfin.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class ItemsPage : Page
    {
        public ItemsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.PageReadyAsync((Guid)e.Parameter);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MediaDataItem item = e.ClickedItem as MediaDataItem;

            App.Current.Shell.ChangeMenuSelection(item.BaseItem.Id);
        }

        private void TitleScrollerCanvas_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TitlePlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected media item
            MediaDataItem MediaDataItem = (MediaDataItem)((Button)sender).DataContext;

            // Set root frame to media player
            App.Current.RootFrame.Navigate(typeof(MediaPlayerPage), MediaDataItem.BaseItem.Id);
        }
    }
}
