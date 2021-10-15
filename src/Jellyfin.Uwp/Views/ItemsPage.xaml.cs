using Jellyfin.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class ItemsPage : Page
    {
        public ItemsPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get UserView
            ViewModel.UserView = App.Current.UserViews.Items.FirstOrDefault(x => x.Id == (Guid)e.Parameter);

            // Set Initial SortBy and SortOrder
            switch (ViewModel.UserView.CollectionType)
            {
                case "movies":
                    SortByList.SelectedItem = ViewModel.SortByCollection.FirstOrDefault(i => i.Value == "SortName");
                    SortOrderList.SelectedItem = ViewModel.SortOrderCollection.FirstOrDefault(i => i.Value == "Descending");
                    break;
                default:
                    SortByList.SelectedItem = ViewModel.SortByCollection.FirstOrDefault(i => i.Value == "SortName");
                    SortOrderList.SelectedItem = ViewModel.SortOrderCollection.FirstOrDefault(i => i.Value == "Ascending");
                    break;
            }

            await ViewModel.PageReadyAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MediaDataItem item = e.ClickedItem as MediaDataItem;

            App.Current.Shell.ChangeMenuSelection(item.BaseItem.Id);
        }

        private void SortByList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Add Set Sort By Logic
            Console.WriteLine();
        }

        private void SortOrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Add Set Sort Order Logic
            Console.WriteLine();
        }
    }
}
