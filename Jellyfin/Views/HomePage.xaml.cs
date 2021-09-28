using Jellyfin.ViewModels;
using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
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

        private void RadSlideHubTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            LibrariesModel library = (LibrariesModel)((RadSlideHubTile)sender).DataContext;

            App.Current.Shell.ChangeMenuSelection(library.Id);
        }

        private void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            LatestMediaItemModel LatestItem = (LatestMediaItemModel)((Grid)sender).DataContext;

            App.Current.Shell.ChangeMenuSelection(LatestItem.Id);

            Frame.Navigate(typeof(ItemPage), LatestItem.Id);
        }
    }
}
