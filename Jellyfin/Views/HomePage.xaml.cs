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

        private void MyMediaBackNav_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //MyMediaList.ScrollIntoView(0); // Go to front of list
        }

        private void MyMediaForwardNav_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //MyMediaList.ScrollIntoView(App.Current.UserViews.Items.Count); //Go to end of list
        }
    }
}
