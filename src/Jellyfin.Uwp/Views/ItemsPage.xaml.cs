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

            ViewModel.IsBusy = true;

            // Get UserView Library
            ViewModel.UserView = App.Current.UserViews.Items.FirstOrDefault(x => x.Id == (Guid)e.Parameter);

            // Instantiate Initial SortByCollection and SortOrderCollection by CollectionType
            switch (ViewModel.UserView.CollectionType)
            {
                case "movies":
                    #region Movies
                    // Instantiate IncludeItemTypesCollection
                    ViewModel.IncludeItemTypesCollection = new ObservableCollection<string>
                    {
                        "Movie"
                    };

                    // Instantiate FieldsCollection
                    ViewModel.FieldsCollection = new ObservableCollection<ItemFields>
                    {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.MediaSourceCount,
                            ItemFields.BasicSyncInfo,
                            ItemFields.Tags
                    };

                    // Instantiate EnableImageTypesCollection
                    ViewModel.EnableImageTypesCollection = new ObservableCollection<ImageType>
                    {
                        ImageType.Primary,
                        ImageType.Backdrop,
                        ImageType.Banner,
                        ImageType.Thumb
                    };

                    ViewModel.Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        parentId: ViewModel.UserView.Id,
                        fields: ViewModel.FieldsCollection.ToArray(),
                        includeItemTypes: ViewModel.IncludeItemTypesCollection.ToArray(),
                        enableImageTypes: ViewModel.EnableImageTypesCollection.ToArray(),
                        recursive:true);

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

                    // Instantiate SelectedSortOrder
                    ViewModel.SelectedSortOrder = new ObservableCollection<SortOrder>() { SortOrder.Descending };

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

                    // Instantiate SelectedSortBy
                    ViewModel.SelectedSortBy = new ObservableCollection<string>()
                        {
                            "DateCreated",
                            "SortName",
                            "ProductionYear"
                        };

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

                    // Instantiate SelectedFilter
                    ViewModel.SelectedFilters = new ObservableCollection<ItemFilter>();

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

                    // Instantiate SelectedGenres
                    ViewModel.SelectedGenres = new ObservableCollection<string>();

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

                    // Instantiate SelectedVideoTypes
                    ViewModel.SelectedVideoTypes = new ObservableCollection<VideoType>();

                    // Instantiate ParentalRatingsCollection
                    List<string> movieParentalRatings = ViewModel.Query.Items.Select(i => i.OfficialRating).Distinct().ToList();
                    if (movieParentalRatings.Count > 0)
                    {
                        ViewModel.ParentalRatingCollection = new ObservableCollection<SortFilterDataItem>();
                        ViewModel.IsParentalRatingsFilterVisible = true;
                        foreach (string item in movieParentalRatings)
                        {
                            ViewModel.ParentalRatingCollection.Add(new SortFilterDataItem
                            {
                                DisplayName = item ?? "None",
                                Value = item ?? "None",
                                IsSelected = false
                            });
                        }
                    }

                    // Instantiate SelectedParentalRatings
                    ViewModel.SelectedParentalRatings = new ObservableCollection<string>();

                    // Instantiate TagsCollection
                    List<string> movieTags = ViewModel.Query.Items.SelectMany(i => i.Tags).Distinct().ToList();
                    ViewModel.TagsCollection = new ObservableCollection<SortFilterDataItem>();
                    ViewModel.IsTagsFilterVisible = true;
                    foreach (string item in movieTags)
                    {
                        ViewModel.TagsCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item,
                            Value = item,
                            IsSelected = false
                        });
                    }

                    // Instantiate SelectedTags
                    ViewModel.SelectedTags = new ObservableCollection<string>();

                    // Instantiate YearsCollection
                    List<int?> movieYears = ViewModel.Query.Items.Select(i => i.ProductionYear).OrderBy(i => i.Value).Distinct().ToList();
                    ViewModel.YearsCollection = new ObservableCollection<SortFilterDataItem>();
                    ViewModel.IsYearsFilterVisible = true;
                    foreach (int? item in movieYears)
                    {
                        ViewModel.YearsCollection.Add(new SortFilterDataItem
                        {
                            DisplayName = item == 0 ? "None" : item.ToString(),
                            Value = item == 0 ? "None" : item.ToString(),
                            IsSelected = false
                        });
                    }

                    // Instantiate SelectedYears
                    ViewModel.SelectedYears = new ObservableCollection<int>();
                    
                    break;
                #endregion
                case "tvshows":
                    #region TV Shows
                    // Instantiate IncludeItemTypesCollection
                    ViewModel.IncludeItemTypesCollection = new ObservableCollection<string>
                    {
                        "Series"
                    };

                    // Instantiate FieldsCollection
                    ViewModel.FieldsCollection = new ObservableCollection<ItemFields>
                    {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.BasicSyncInfo,
                            ItemFields.Tags
                    };

                    // Instantiate EnableImageTypesCollection
                    ViewModel.EnableImageTypesCollection = new ObservableCollection<ImageType>
                    {
                        ImageType.Primary,
                        ImageType.Backdrop,
                        ImageType.Banner,
                        ImageType.Thumb
                    };

                    ViewModel.Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        parentId: ViewModel.UserView.Id,
                        fields: ViewModel.FieldsCollection.ToArray(),
                        includeItemTypes: ViewModel.IncludeItemTypesCollection.ToArray(),
                        enableImageTypes: ViewModel.EnableImageTypesCollection.ToArray(),
                        recursive: true);

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

                    // Instantiate SelectedSortOrder
                    ViewModel.SelectedSortOrder = new ObservableCollection<SortOrder> { SortOrder.Ascending };

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

                    // Instantiate SelectedSortBy
                    ViewModel.SelectedSortBy = new ObservableCollection<string> { "SortName" };

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

                    // Instantiate SelectedFilter
                    ViewModel.SelectedFilters = new ObservableCollection<ItemFilter>();

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

                    // Instantiate SelectedStatus
                    ViewModel.SelectedSeriesStatus = new ObservableCollection<SeriesStatus>();

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

                    // Instantiate SelectedGenres
                    ViewModel.SelectedGenres = new ObservableCollection<string>();

                    // Instantiate ParentalRatings Collection
                    List<string> tvShowParentalRatings = ViewModel.Query.Items.Select(i => i.OfficialRating).Distinct().ToList();
                    if (tvShowParentalRatings.Count > 0)
                    {
                        ViewModel.ParentalRatingCollection = new ObservableCollection<SortFilterDataItem>();
                        ViewModel.IsParentalRatingsFilterVisible = true;
                        foreach (string item in tvShowParentalRatings)
                        {
                            ViewModel.ParentalRatingCollection.Add(new SortFilterDataItem
                            {
                                DisplayName = item ?? "None",
                                Value = item ?? "None",
                                IsSelected = false
                            });
                        }
                    }

                    // Instantiate SelectedParentalRatings
                    ViewModel.SelectedParentalRatings = new ObservableCollection<string>();

                    // Instantiate TagsCollection
                    List<string> tvShowTags = ViewModel.Query.Items.SelectMany(i => i.Tags).Distinct().ToList();
                    if (tvShowTags.Count > 0)
                    {
                        ViewModel.TagsCollection = new ObservableCollection<SortFilterDataItem>();
                        ViewModel.IsTagsFilterVisible = true;
                        foreach (string item in tvShowTags)
                        {
                            ViewModel.TagsCollection.Add(new SortFilterDataItem
                            {
                                DisplayName = item,
                                Value = item,
                                IsSelected = false
                            });
                        }
                    }

                    // Instantiate SelectedTags
                    ViewModel.SelectedTags = new ObservableCollection<string>();

                    // Instantiate YearsCollection
                    List<int?> tvShowYears = ViewModel.Query.Items.Select(i => i.ProductionYear != null ? i.ProductionYear : 0).Distinct().OrderBy(i => i.Value).ToList();
                    if (tvShowYears.Count > 0)
                    {
                        ViewModel.YearsCollection = new ObservableCollection<SortFilterDataItem>();
                        ViewModel.IsYearsFilterVisible = true;
                        foreach (int? item in tvShowYears)
                        {
                            ViewModel.YearsCollection.Add(new SortFilterDataItem
                            {
                                DisplayName = item == 0 ? "None" : item.ToString(),
                                Value = item == 0 ? "None" : item.ToString(),
                                IsSelected = false
                            });
                        }
                    }

                    // Instantiate SelectedYears
                    ViewModel.SelectedYears = new ObservableCollection<int>();

                    break;
                #endregion
                case "music":
                    #region Music
                    // Instantiate IncludeItemTypesCollection
                    ViewModel.IncludeItemTypesCollection = new ObservableCollection<string>
                    {
                        "MusicAlbum"
                    };

                    // Instantiate FieldsCollection
                    ViewModel.FieldsCollection = new ObservableCollection<ItemFields>
                    {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.SortName,
                            ItemFields.BasicSyncInfo,
                    };

                    // Instantiate EnableImageTypesCollection
                    ViewModel.EnableImageTypesCollection = new ObservableCollection<ImageType>
                    {
                        ImageType.Primary,
                        ImageType.Backdrop,
                        ImageType.Banner,
                        ImageType.Thumb
                    };

                    ViewModel.Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        parentId: ViewModel.UserView.Id,
                        fields: ViewModel.FieldsCollection.ToArray(),
                        includeItemTypes: ViewModel.IncludeItemTypesCollection.ToArray(),
                        enableImageTypes: ViewModel.EnableImageTypesCollection.ToArray(),
                        recursive: true);

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

                    // Instantiate SelectedSortOrder
                    ViewModel.SelectedSortOrder = new ObservableCollection<SortOrder> { SortOrder.Ascending };

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

                    // Instantiate SelectedSortBy
                    ViewModel.SelectedSortBy = new ObservableCollection<string> { "SortName" };

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

                    // Instantiate SelectedFilters
                    ViewModel.SelectedFilters = new ObservableCollection<ItemFilter>();

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

                    // Instantiate SelectedGenres
                    ViewModel.SelectedGenres = new ObservableCollection<string>();

                    // Instantiate YearsCollection
                    List<int?> musicYears = ViewModel.Query.Items.Select(i => i.ProductionYear != null ? i.ProductionYear : 0).Distinct().OrderBy(i => i.Value).ToList();
                    if (musicYears.Count > 0)
                    {
                        ViewModel.YearsCollection = new ObservableCollection<SortFilterDataItem>();
                        ViewModel.IsYearsFilterVisible = true;
                        foreach (int? item in musicYears)
                        {
                            ViewModel.YearsCollection.Add(new SortFilterDataItem
                            {
                                DisplayName = item == 0 ? "None" : item.ToString(),
                                Value = item == 0 ? "None" : item.ToString(),
                                IsSelected = false
                            });
                        }
                    }

                    // Instantiate SelectedYears
                    ViewModel.SelectedYears = new ObservableCollection<int>();

                    break;

                #endregion
                default:
                    #region Audiobooks, Photos/Home Videos, Collections
                    // Instantiate FieldsCollection
                    ViewModel.FieldsCollection = new ObservableCollection<ItemFields>
                    {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.SortName,
                            ItemFields.Path,
                    };

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

                    // Instantiate SelectedSortOrder
                    ViewModel.SelectedSortOrder = new ObservableCollection<SortOrder> { SortOrder.Ascending };

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

                    // Instantiate SelectedSortBy
                    ViewModel.SelectedSortBy = new ObservableCollection<string> { "IsFolder", "SortName" };

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

                    // Instantiate SelectedFilters
                    ViewModel.SelectedFilters = new ObservableCollection<ItemFilter>();

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

                    // Instantiate SelectedVideoTypes
                    ViewModel.SelectedVideoTypes = new ObservableCollection<VideoType>();

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

                    break;
                    #endregion
            }

            await ViewModel.PageReadyAsync();

            ViewModel.IsBusy = false;
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

            foreach (SortFilterDataItem item in ViewModel.FilterCollection.Where(i => i.IsSelected))
            {

            }
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

        private void VideoTypesCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void YearsCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private async void ApplySortButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.PageReadyAsync();
        }

        private async void ApplyFilterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.PageReadyAsync();
        }
    }
}
