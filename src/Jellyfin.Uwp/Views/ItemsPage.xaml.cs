using Jellyfin.Models;
using Jellyfin.Models.Enums;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                        ViewModel.SortOrderCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Descending"
                        });
                    }

                    // Intantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(MoviesSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortFilterDataItem
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

                    // Instantiate FilterCollection
                    foreach (object item in Enum.GetValues(typeof(MoviesFilters)))
                    {
                        ViewModel.FilterCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                            .GetField(item.ToString())
                                            .GetCustomAttribute<DisplayNameAttribute>()
                                            .DisplayName,
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }

                    // Instantiate FeaturesCollection
                    ViewModel.IsFeaturesFilterVisible = true;
                    ViewModel.FeaturesCollection = new ObservableCollection<SortFilterDataItem>();
                    foreach (object item in Enum.GetValues(typeof(FeaturesFilters)))
                    {
                        ViewModel.FeaturesCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                            .GetField(item.ToString())
                                            .GetCustomAttribute<DisplayNameAttribute>()
                                            .DisplayName,
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }

                    // Instantiate GenresCollection
                    ViewModel.IsGenresFilterVisible = true;
                    ViewModel.GenresCollection = new ObservableCollection<SortFilterDataItem>();
                    JellyfinClientServices.Current.GenresClient = new GenresClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
                    BaseItemDtoQueryResult MovieGenres = await JellyfinClientServices.Current.GenresClient.GetGenresAsync(parentId: ViewModel.UserView.Id);

                    foreach (BaseItemDto item in MovieGenres.Items)
                    {
                        ViewModel.GenresCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.Name,
                            Value = item.Name,
                            IsSelected = false
                        });
                    }

                    // Instantiate VideoTypesCollection
                    ViewModel.IsVideoTypesFilterVisible = true;
                    ViewModel.VideoTypesCollection = new ObservableCollection<SortFilterDataItem>();
                    foreach (object item in Enum.GetValues(typeof(VideoTypeFilters)))
                    {
                        ViewModel.VideoTypesCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                            .GetField(item.ToString())
                                            .GetCustomAttribute<DisplayNameAttribute>()
                                            .DisplayName,
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }


                    break;
                case "tvshows":
                    // Instantiate SortOrderCollection
                    foreach (object item in Enum.GetValues(typeof(SortOrder)))
                    {
                        ViewModel.SortOrderCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Ascending"
                        });
                    }

                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(TvShowsSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "SortName"
                        });
                    }

                    // Instantiate FilterCollection
                    foreach (object item in Enum.GetValues(typeof(TvShowsFilters)))
                    {
                        ViewModel.FilterCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                            .GetField(item.ToString())
                                            .GetCustomAttribute<DisplayNameAttribute>()
                                            .DisplayName,
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }


                    // Instantiate StatusCollection
                    ViewModel.IsStatusFilterVisible = true;
                    ViewModel.SeriesStatusCollection = new ObservableCollection<SortFilterDataItem>();
                    foreach (object item in Enum.GetValues(typeof(SeriesStatus)))
                    {
                        ViewModel.SeriesStatusCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }

                    // Instantiate FeaturesCollection
                    ViewModel.IsFeaturesFilterVisible = true;
                    ViewModel.FeaturesCollection = new ObservableCollection<SortFilterDataItem>();
                    foreach (object item in Enum.GetValues(typeof(FeaturesFilters)))
                    {
                        ViewModel.FeaturesCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                            .GetField(item.ToString())
                                            .GetCustomAttribute<DisplayNameAttribute>()
                                            .DisplayName,
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }

                    // Instantiate GenresCollection
                    ViewModel.IsGenresFilterVisible = true;
                    ViewModel.GenresCollection = new ObservableCollection<SortFilterDataItem>();
                    JellyfinClientServices.Current.GenresClient = new GenresClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
                    BaseItemDtoQueryResult TvShowsGenres = await JellyfinClientServices.Current.GenresClient.GetGenresAsync(parentId: ViewModel.UserView.Id);

                    foreach (BaseItemDto item in TvShowsGenres.Items)
                    {
                        ViewModel.GenresCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.Name,
                            Value = item.Name,
                            IsSelected = false
                        });
                    }

                    break;
                case "music":
                    // Instantiate SortOrderCollection
                    foreach (object item in Enum.GetValues(typeof(SortOrder)))
                    {
                        ViewModel.SortOrderCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Ascending"
                        });
                    }

                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(MusicSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "SortName"
                        });
                    }

                    // Instantiate FilterCollection
                    foreach (object item in Enum.GetValues(typeof(MusicFilters)))
                    {
                        ViewModel.FilterCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                            .GetField(item.ToString())
                                            .GetCustomAttribute<DisplayNameAttribute>()
                                            .DisplayName,
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }

                    // Instantiate GenresCollection
                    ViewModel.IsGenresFilterVisible = true;
                    ViewModel.GenresCollection = new ObservableCollection<SortFilterDataItem>();
                    JellyfinClientServices.Current.GenresClient = new GenresClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
                    BaseItemDtoQueryResult MusicGenres = await JellyfinClientServices.Current.GenresClient.GetGenresAsync(parentId: ViewModel.UserView.Id);

                    foreach (BaseItemDto item in MusicGenres.Items)
                    {
                        ViewModel.GenresCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.Name,
                            Value = item.Name,
                            IsSelected = false
                        });
                    }

                    break;
                default:
                    // Instantiate SortOrderCollection
                    foreach (object item in Enum.GetValues(typeof(SortOrder)))
                    {
                        ViewModel.SortOrderCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.ToString(),
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "Ascending"
                        });
                    }

                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(FolderSortBy)))
                    {
                        ViewModel.SortByCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString(),
                            IsSelected = item.ToString() == "IsFolder" || item.ToString() == "SortName"
                        });
                    }

                    // Instantiate FilterCollection
                    foreach (object item in Enum.GetValues(typeof(FolderFilters)))
                    {
                        ViewModel.FilterCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item.GetType()
                                            .GetField(item.ToString())
                                            .GetCustomAttribute<DisplayNameAttribute>()
                                            .DisplayName,
                            Value = item.ToString(),
                            IsSelected = false
                        });
                    }

                    // Instantiate VideoTypesCollection
                    ViewModel.IsVideoTypesFilterVisible = true;

                    // Instantiate FeaturesCollection
                    ViewModel.IsFeaturesFilterVisible = true;

                    break;
            }

            await ViewModel.PageReadyAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MediaDataItem item = e.ClickedItem as MediaDataItem;

            App.Current.Shell.ChangeMenuSelection(item.BaseItem.Id);
        }

        private void SortByCheckBox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)checkBox.DataContext;

            SortFilterDataItem updateItem = ViewModel.SortByCollection.First(i => i.Value == dataItem.Value);

            updateItem.IsSelected = !dataItem.IsSelected;
        }

        private void SortOrderRadioButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            SortFilterDataItem dataItem = (SortFilterDataItem)radioButton.DataContext;

            foreach (SortFilterDataItem updateItem in ViewModel.SortOrderCollection)
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
        }

        private void FilterCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)checkBox.DataContext;

            SortFilterDataItem updateItem = ViewModel.FilterCollection.First(i => i.Value == dataItem.Value);

            updateItem.IsSelected = !dataItem.IsSelected;
        }

        private async void ApplySortButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.PageReadyAsync();
        }

        private async void ApplyFilterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.PageReadyAsync();
        }

        private void StatusCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void FeaturesCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void GenresCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void ParentalRatingsCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void TagsCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void VideoTYpesCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void YearsCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
