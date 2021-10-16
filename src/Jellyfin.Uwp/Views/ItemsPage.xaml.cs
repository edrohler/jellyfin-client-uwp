using Jellyfin.Models;
using Jellyfin.Models.Enums;
using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

            // Get UserView Library
            ViewModel.UserView = App.Current.UserViews.Items.FirstOrDefault(x => x.Id == (Guid)e.Parameter);

            // Instantiate Initial SortByCollection and SortOrderCollection by CollectionType
            switch (ViewModel.UserView.CollectionType)
            {
                case "movies":
                    // Instantiate SortOrderCollection
                    foreach (object item in Enum.GetValues(typeof(SortOrder)))
                    {
                        ViewModel.SortOrderCollection.Add(new SortDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Descending"
                        });
                    }

                    // Intantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(MoviesSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "DateCreated"
                                    || item.ToString() == "SortName"
                                    || item.ToString() == "ProductionYear"
                        });
                    }
                    break;
                case "tvshows":
                    // Instantiate SortOrderCollection
                    foreach (object item in Enum.GetValues(typeof(SortOrder)))
                    {
                        ViewModel.SortOrderCollection.Add(new SortDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Ascending"
                        });
                    }

                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(TvShowsSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "SortName"
                        });
                    }
                    break;
                case "music":
                    // Instantiate SortOrderCollection
                    foreach (object item in Enum.GetValues(typeof(SortOrder)))
                    {
                        ViewModel.SortOrderCollection.Add(new SortDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Ascending"
                        });
                    }

                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(MusicSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "SortName"
                        });
                    }
                    break;
                default:
                    // Instantiate SortOrderCollection
                    foreach (object item in Enum.GetValues(typeof(SortOrder)))
                    {
                        ViewModel.SortOrderCollection.Add(new SortDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Ascending"
                        });
                    }

                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(FolderSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "IsFolder" || item.ToString() == "SortName"
                        });
                    }
                    break;
            }

            await ViewModel.PageReadyAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MediaDataItem item = e.ClickedItem as MediaDataItem;

            App.Current.Shell.ChangeMenuSelection(item.BaseItem.Id);
        }

        private async void SortByCheckBox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortDataItem dataItem = (SortDataItem)checkBox.DataContext;

            SortDataItem updateItem = ViewModel.SortByCollection.First(i => i.Value == dataItem.Value);

            updateItem.IsSelected = !dataItem.IsSelected;

            await ViewModel.PageReadyAsync();
        }

        private async void SortOrderRadioButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            SortDataItem dataItem = (SortDataItem)radioButton.DataContext;

            foreach (SortDataItem updateItem in ViewModel.SortOrderCollection)
            {
                if (updateItem.Value == dataItem.Value)
                {
                    updateItem.IsSelected = !dataItem.IsSelected;
                }
                else
                {
                    updateItem.IsSelected = false;
                }
            }

            await ViewModel.PageReadyAsync();
        }
    }
}
