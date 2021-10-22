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
                            IsSelected = item.ToString() == "Ascending"
                        });
                    }

                    // Instantiate SelectedSortOrder
                    ViewModel.SelectedSortOrder = new ObservableCollection<SortOrder>() { SortOrder.Ascending };

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
                            IsSelected = item.ToString() == "SortName"
                        });
                    }

                    // Instantiate SelectedSortBy
                    ViewModel.SelectedSortBy = new ObservableCollection<string>()
                        {
                            "SortName"
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

                    // Instantiate ParentalRatingsCollection
                    List<string> movieParentalRatings = ViewModel.Query.Items.Select(i => i.OfficialRating).Distinct().ToList();
                    if (movieParentalRatings.Count > 0)
                    {
                        ViewModel.ParentalRatingCollection = new ObservableCollection<SortFilterDataItem>();
                        ViewModel.IsParentalRatingsFilterVisible = true;
                        foreach (string item in movieParentalRatings)
                        {
                            if(!string.IsNullOrEmpty(item))
                            {
                                ViewModel.ParentalRatingCollection.Add(new SortFilterDataItem
                                {
                                    DisplayName = item,
                                    Value = item,
                                    IsSelected = false
                                });
                            }
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
                    List<int?> movieYears = ViewModel.Query.Items.Select(i => i.ProductionYear != null ? i.ProductionYear : 0).OrderBy(i => i.Value).Distinct().ToList();
                    ViewModel.YearsCollection = new ObservableCollection<SortFilterDataItem>();
                    ViewModel.IsYearsFilterVisible = true;
                    foreach (int? item in movieYears)
                    {
                        if (item != 0)
                        {
                            ViewModel.YearsCollection.Add(new SortFilterDataItem
                            {
                                DisplayName = item.ToString(),
                                Value = item.ToString(),
                                IsSelected = false
                            });
                        }
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
                            if (!string.IsNullOrEmpty(item))
                            {
                                ViewModel.ParentalRatingCollection.Add(new SortFilterDataItem
                                {
                                    DisplayName = item,
                                    Value = item,
                                    IsSelected = false
                                });
                            }
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
                            if (item != 0)
                            {
                                ViewModel.YearsCollection.Add(new SortFilterDataItem
                                {
                                    DisplayName = item.ToString(),
                                    Value = item.ToString(),
                                    IsSelected = false
                                });
                            }
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
                            if (item != 0)
                            {
                                ViewModel.YearsCollection.Add(new SortFilterDataItem
                                {
                                    DisplayName = item.ToString(),
                                    Value = item.ToString(),
                                    IsSelected = false
                                });
                            }
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
                            IsSelected = item.ToString() == "SortName"
                        });
                    }

                    // Instantiate SelectedSortBy
                    ViewModel.SelectedSortBy = new ObservableCollection<string> { "SortName" };

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

                    break;
                    #endregion
            }

            await ViewModel.PageReadyAsync();

            ViewModel.IsBusy = false;
        }

        #region UI Interaction Events
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

            ViewModel.SelectedSortBy.Clear();
            foreach (SortFilterDataItem item in ViewModel.SortByCollection.Where(i => i.IsSelected))
            {
                ViewModel.SelectedSortBy.Add(item.Value);
            }
        }

        private void SortOrderRadioButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)radioButton.DataContext;

            foreach (SortFilterDataItem updateItem in ViewModel.SortOrderCollection)
            {
                updateItem.IsSelected = updateItem.Value == dataItem.Value && !dataItem.IsSelected;
            }

            ViewModel.SelectedSortOrder.Clear();
            switch (dataItem.Value)
            {
                case "Ascending":
                    ViewModel.SelectedSortOrder.Add(SortOrder.Ascending);
                    break;
                case "Descending":
                    ViewModel.SelectedSortOrder.Add(SortOrder.Descending);
                    break;
                default:
                    break;
            }
        }

        private void FilterCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)checkBox.DataContext;

            SortFilterDataItem updateItem = ViewModel.FilterCollection.First(i => i.Value == dataItem.Value);
            updateItem.IsSelected = !dataItem.IsSelected;

            ViewModel.SelectedFilters.Clear();
            foreach (SortFilterDataItem item in ViewModel.FilterCollection.Where(i => i.IsSelected))
            {
                switch (item.Value)
                {
                    case "IsResumable":
                        ViewModel.SelectedFilters.Add(ItemFilter.IsResumable);
                        break;
                    case "IsFavorite":
                        ViewModel.SelectedFilters.Add(ItemFilter.IsFavorite);
                        break;
                    case "IsPlayed":
                        ViewModel.SelectedFilters.Add(ItemFilter.IsPlayed);
                        break;
                    case "IsUnplayed":
                        ViewModel.SelectedFilters.Add(ItemFilter.IsUnplayed);
                        break;
                    case "Likes":
                        ViewModel.SelectedFilters.Add(ItemFilter.Likes);
                        break;
                    case "Dislikes":
                        ViewModel.SelectedFilters.Add(ItemFilter.Dislikes);
                        break;
                    default:
                        break;
                }
            }
        }

        private void StatusCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)checkBox.DataContext;

            SortFilterDataItem updateItem = ViewModel.SeriesStatusCollection.First(i => i.Value == dataItem.Value);
            updateItem.IsSelected = !dataItem.IsSelected;

            ViewModel.SelectedSeriesStatus.Clear();
            foreach (SortFilterDataItem item in ViewModel.SeriesStatusCollection.Where(i => i.IsSelected))
            {
                switch (item.Value)
                {
                    case "Continuing":
                        ViewModel.SelectedSeriesStatus.Add(SeriesStatus.Continuing);
                        break;
                    case "Ended":
                        ViewModel.SelectedSeriesStatus.Add(SeriesStatus.Ended);
                        break;
                    default:
                        break;
                }
            }
        }

        private void GenresCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)checkBox.DataContext;

            SortFilterDataItem updateItem = ViewModel.GenresCollection.First(i => i.Value == dataItem.Value);
            updateItem.IsSelected = !dataItem.IsSelected;

            ViewModel.SelectedGenres.Clear();
            foreach (SortFilterDataItem item in ViewModel.GenresCollection.Where(i => i.IsSelected))
            {
                ViewModel.SelectedGenres.Add(item.Value);
            }
        }

        private void ParentalRatingsCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)checkBox.DataContext;

            SortFilterDataItem updateItem = ViewModel.ParentalRatingCollection.First(i => i.Value == dataItem.Value);
            updateItem.IsSelected = !dataItem.IsSelected;

            ViewModel.ParentalRatingCollection.Clear();
            foreach (SortFilterDataItem item in ViewModel.ParentalRatingCollection.Where(i => i.IsSelected))
            {
                if (item.Value != "None")
                {
                    ViewModel.ParentalRatingCollection.Add(item);
                }
            }
        }

        private void TagsCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void YearsCheckbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            SortFilterDataItem dataItem = (SortFilterDataItem)checkBox.DataContext;

            SortFilterDataItem updateItem = ViewModel.YearsCollection.First(i => i.Value == dataItem.Value);
            updateItem.IsSelected = !dataItem.IsSelected;

            ViewModel.SelectedYears.Clear();
            foreach (SortFilterDataItem item in ViewModel.YearsCollection.Where(i => i.IsSelected))
            {
                bool isYear = int.TryParse(item.Value, out int year);
                if (isYear)
                {
                    ViewModel.SelectedYears.Add(year);
                }
            }
        }

        private async void ApplySortButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.PageReadyAsync();
        }

        private async void ApplyFilterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.PageReadyAsync();
        }

        #endregion
    }
}
