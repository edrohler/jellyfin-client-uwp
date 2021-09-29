using Jellyfin.Models;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);

           await ViewModel.PageReadyAsync();
        }

        private void MyMediaHubtile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            LibraryDataItems library = (LibraryDataItems)((RadSlideHubTile)sender).DataContext;

            App.Current.Shell.ChangeMenuSelection(library.Id);
        }

        private void LatestMediaHubTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            LatestMediaDataItem LatestItem = (LatestMediaDataItem)((RadHubTile)sender).DataContext;

            App.Current.Shell.ChangeMenuSelection(LatestItem.Id);

            Frame.Navigate(typeof(ItemPage), LatestItem.Id);
        }
    }
}
