using Jellyfin.Models;
using Jellyfin.ViewModels;
using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class HomePage : Page
    {
        //private DispatcherTimer timer = null;

        public HomePage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);

            //timer = new DispatcherTimer();

           await ViewModel.PageReadyAsync();
        }


        // Navigate to Library Page
        private void MyMediaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaDataItem SelectedItem = (MediaDataItem)((ListView)sender).SelectedItem;

            App.Current.Shell.ChangeMenuSelection(SelectedItem.BaseItem.Id);
        }

        // Navigate to Media Item Page
        private void LatestMediaItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MediaDataItem SelectedItem = (MediaDataItem)((ListView)sender).SelectedItem;

            NavigateToItemPage(SelectedItem.BaseItem.Id);
        }

        private void NavigateToItemPage(Guid id)
        {
            App.Current.Shell.ChangeMenuSelection(id);

            Frame.Navigate(typeof(ItemPage), id);
        }

        // Play Media Item
        private void TitlePlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected media item
            MediaDataItem MediaDataItem = (MediaDataItem)((Button)sender).DataContext;

            // Set root frame to media player
            App.Current.RootFrame.Navigate(typeof(MediaPlayerPage), MediaDataItem.BaseItem.Id);
        }
    }
}
