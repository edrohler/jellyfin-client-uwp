using Jellyfin.Models;
using Jellyfin.Models.Enums;
using Jellyfin.Sdk;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get UserView
            ViewModel.UserView = App.Current.UserViews.Items.FirstOrDefault(x => x.Id == (Guid)e.Parameter);

            // Instantiate SortOrderCollection
            foreach (object item in Enum.GetValues(typeof(SortOrder)))
            {
                ViewModel.SortOrderCollection.Add(new SortDataItem
                {
                    DisplayName = item.ToString(),
                    Value = item.ToString()
                });
            }

            // Instantiate SortByCollection
            switch (ViewModel.UserView.CollectionType)
            {
                case "movies":
                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(MoviesSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    ViewModel.SortOrder = new SortOrder[] { SortOrder.Descending };
                    ViewModel.SortBy = new string[] { "DateCreated", "SortName", "ProductionYear" };
                    SortByList.SelectedItem = ViewModel.SortByCollection.FirstOrDefault(i => i.Value == "SortName");
                    SortOrderList.SelectedItem = ViewModel.SortOrderCollection.FirstOrDefault(i => i.Value == "Descending");
                    break;
                case "tvshows":
                    foreach (object item in Enum.GetValues(typeof(TvShowsSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    ViewModel.SortOrder = new SortOrder[] { SortOrder.Ascending };
                    ViewModel.SortBy = new string[] { "SortName" };
                    SortByList.SelectedItem = ViewModel.SortByCollection.FirstOrDefault(i => i.Value == "SortName");
                    SortOrderList.SelectedItem = ViewModel.SortOrderCollection.FirstOrDefault(i => i.Value == "Ascending");
                    break;
                case "music":
                    foreach (object item in Enum.GetValues(typeof(MusicSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    ViewModel.SortOrder = new SortOrder[] { SortOrder.Ascending };
                    ViewModel.SortBy = new string[] { "SortName" };
                    SortByList.SelectedItem = ViewModel.SortByCollection.FirstOrDefault(i => i.Value == "SortName");
                    SortOrderList.SelectedItem = ViewModel.SortOrderCollection.FirstOrDefault(i => i.Value == "Ascending");
                    break;
                default:
                    foreach (object item in Enum.GetValues(typeof(FolderSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    ViewModel.SortOrder = new SortOrder[] { SortOrder.Ascending };
                    ViewModel.SortBy = new string[] { "IsFolder", "SortName" };
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
