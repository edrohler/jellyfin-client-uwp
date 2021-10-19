using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Helpers;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        private int totalCount;
        private int startIndex;
        private int limit;
        private bool isPageable;
        private bool backButtonIsEnabled;
        private bool nextButtonIsEnabled;
        private bool isStatusFilterVisible;
        private bool isFeaturesFilterVisible;
        private bool isGenresFilterVisible;
        private bool isParentalRatingsFilterVisible;
        private bool isTagsFilterVisible;
        private bool isVideoTypesFilterVisible;
        private bool isYearsFilterVisible;
        private string pageStatusString;
        private BaseItemDtoQueryResult Query;
        private BaseItemDto userView;

        public int TotalCount { get => totalCount; set => SetProperty(ref totalCount, value); }
        public int StartIndex { get => startIndex; set => SetProperty(ref startIndex, value); }
        public int Limit { get => limit; set => SetProperty(ref limit, value); }
        public bool IsPageable { get => isPageable; set => SetProperty(ref isPageable, value); }
        public bool BackButtonIsEnabled { get => backButtonIsEnabled; set => SetProperty(ref backButtonIsEnabled, value); }
        public bool NextButtonIsEnabled { get => nextButtonIsEnabled; set => SetProperty(ref nextButtonIsEnabled, value); }
        public bool IsStatusFilterVisible { get => isStatusFilterVisible; set => SetProperty(ref isStatusFilterVisible, value); }
        public bool IsFeaturesFilterVisible { get => isFeaturesFilterVisible; set => SetProperty(ref isFeaturesFilterVisible, value); }
        public bool IsGenresFilterVisible { get => isGenresFilterVisible; set => SetProperty(ref isGenresFilterVisible, value); }
        public bool IsParentalRatingsFilterVisible { get => isParentalRatingsFilterVisible; set => SetProperty(ref isParentalRatingsFilterVisible, value); }
        public bool IsTagsFilterVisible { get => isTagsFilterVisible; set => SetProperty(ref isTagsFilterVisible, value); }
        public bool IsVideoTypesFIlterVisible { get => isVideoTypesFilterVisible; set => SetProperty(ref isVideoTypesFilterVisible, value); }
        public bool IsYearsFilterVisible { get => isYearsFilterVisible; set => SetProperty(ref isYearsFilterVisible, value); }
        public string PageStatusString { get => pageStatusString; set => SetProperty(ref pageStatusString, value); }
        public BaseItemDto UserView { get => userView; set => SetProperty(ref userView, value); }

        public ObservableCollection<MediaDataItem> GridItems { get; set; }
        public ObservableCollection<SortDataItem> SortByCollection { get; set; }
        public ObservableCollection<SortDataItem> SortOrderCollection { get; set; }
        public ObservableCollection<FilterDataItem> FilterCollection { get; set; }
        public DelegateCommand NextPageCommand { get; set; }
        public DelegateCommand PrevPageCommand { get; set; }

        public ItemsViewModel()
        {
            StartIndex = 0;
            Limit = 100;
            GridItems = new ObservableCollection<MediaDataItem>();
            SortByCollection = new ObservableCollection<SortDataItem>();
            SortOrderCollection = new ObservableCollection<SortDataItem>();
            FilterCollection = new ObservableCollection<FilterDataItem>();
            NextPageCommand = new DelegateCommand(async () => await NextPageAsync());
            PrevPageCommand = new DelegateCommand(async () => await PrevPageAsync());
        }

        public async Task PageReadyAsync()
        {
            // Load Library Items for UserView
            await LoadLibraryItemsAsync();
        }

        public async Task NextPageAsync()
        {
            StartIndex += Limit;
            await LoadLibraryItemsAsync();
        }

        public async Task PrevPageAsync()
        {
            StartIndex -= Limit;
            await LoadLibraryItemsAsync();
        }

        public async Task LoadLibraryItemsAsync()
        {
            IsBusy = true;
            IsBusyMessage = "Loading Content..";

            if (GridItems.Count > 0)
            {
                GridItems.Clear();
            }

            switch (UserView.CollectionType)
            {
                case "music":
                    // Gets Music Library Items
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: GetSortBys(),
                        sortOrder: GetSortOrders(),
                        includeItemTypes: new string[]
                        {
                            "MusicAlbum"
                        },
                        recursive: true,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.SortName,
                            ItemFields.BasicSyncInfo
                        },
                        imageTypeLimit: 1,
                        enableImageTypes: new ImageType[]
                        {
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Banner,
                            ImageType.Thumb
                        },
                        startIndex: StartIndex,
                        limit: Limit,
                        parentId: UserView.Id, 
                        filters: GetItemFilters());
                    UpdatePaging();
                    IsPageable = true;
                    break;
                case "tvshows":
                    // Get TV Shows Library Items
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: GetSortBys(),
                        sortOrder: GetSortOrders(),
                        includeItemTypes: new string[]
                        {
                            "Series"
                        },
                        recursive: true,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.BasicSyncInfo
                        },
                        imageTypeLimit: 1,
                        enableImageTypes: new ImageType[]
                        {
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Banner,
                            ImageType.Thumb
                        },
                        startIndex: StartIndex,
                        limit: Limit,
                        parentId: UserView.Id,
                        filters: GetItemFilters());
                    UpdatePaging();
                    IsPageable = true;
                    break;
                case "movies":
                    // Get Movies Library Items
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: GetSortBys(),
                        sortOrder: GetSortOrders(),
                        includeItemTypes: new string[]
                        {
                            "Movie"
                        },
                        recursive: true,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.MediaSourceCount,
                            ItemFields.BasicSyncInfo
                        },
                        imageTypeLimit: 1,
                        enableImageTypes: new ImageType[]
                        {
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Banner,
                            ImageType.Thumb
                        },
                        startIndex: StartIndex,
                        limit: Limit,
                        parentId: UserView.Id,
                        filters: GetItemFilters());
                    UpdatePaging();
                    IsPageable = true;
                    break;
                default:
                    // Get Audiobooks, Photos/Home Videos and Collections Items
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.SortName,
                            ItemFields.Path
                        },
                        imageTypeLimit: 1,
                        parentId: UserView.Id,
                        sortBy: GetSortBys(),
                        sortOrder: GetSortOrders(),
                        filters: GetItemFilters());
                    IsPageable = false;
                    break;
            }

            if (Query != null)
            {
                // Creates GridView Collection
                foreach (BaseItemDto item in Query.Items)
                {
                    int height, width;

                    if (item.Type == "Series" ||
                        item.Type == "Movie")
                    {
                        height = 450;
                        width = 300;
                    }
                    else
                    {
                        height = 300;
                        width = 300;
                    }

                    BitmapImage img = new BitmapImage
                    {
                        DecodePixelHeight = height,
                        DecodePixelWidth = width
                    };

                    try
                    {
                        FileResponse fr = await JellyfinClientServices.Current.ImageClient.GetItemImageAsync(item.Id, ImageType.Primary);

                        using (Stream stream = fr.Stream)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                await stream.CopyToAsync(ms);
                                ms.Position = 0;

                                await img.SetSourceAsync(ms.AsRandomAccessStream());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO Load default image
                        ExceptionLogger.LogException(ex);
                    }

                    GridItems.Add(new MediaDataItem
                    {
                        BaseItem = item,
                        ImageSource = img,
                        Width = width,
                        Height = height
                    });
                }
            }

            IsBusy = false;
            IsBusyMessage = "";
        }

        private ItemFilter[] GetItemFilters()
        {
            IEnumerable<FilterDataItem> SelectedFilters = FilterCollection.Where(i => i.IsSelected);

            ItemFilter[] filters = new ItemFilter[SelectedFilters.Count()];

            for (int i = 0; i < SelectedFilters.Count(); i++)
            {
                switch (SelectedFilters.ElementAt(i).Value)
                {
                    case "IsUnPlayed":
                        filters[i] = ItemFilter.IsUnplayed;
                        break;
                    case "IsPlayed":
                        filters[i] = ItemFilter.IsPlayed;
                        break;
                    case "IsFavorite":
                        filters[i] = ItemFilter.IsFavorite;
                        break;
                    case "IsResumable":
                        filters[i] = ItemFilter.IsResumable;
                        break;
                    case "Likes":
                        filters[i] = ItemFilter.Likes;
                        break;
                    case "Dislikes":
                        filters[i] = ItemFilter.Dislikes;
                        break;
                }
            }

            return filters;
        }

        private string[] GetSortBys()
        {
            IEnumerable<SortDataItem> SelectedSortBys = SortByCollection.Where(i => i.IsSelected);
            string[] sortBys = new string[SelectedSortBys.Count() + 1];

            for (int i = 0; i < SelectedSortBys.Count(); i++)
            {
                sortBys[i] = SelectedSortBys.ElementAt(i).Value;
            }

            sortBys[SelectedSortBys.Count()] = "ProductionYear";

            return sortBys;
        }

        private SortOrder[] GetSortOrders()
        {
            IEnumerable<SortDataItem> SelectedSortOrders = SortOrderCollection.Where(i => i.IsSelected);
            SortOrder[] sortOrders = new SortOrder[1];

            for (int i = 0; i < SelectedSortOrders.Count(); i++)
            {
                if (SelectedSortOrders.ElementAt(i).Value == "Ascending")
                {
                    sortOrders[i] = SortOrder.Ascending;
                }
                else
                {
                    sortOrders[i] = SortOrder.Descending;
                }
            }

            return sortOrders;
        }

        private void UpdatePaging()
        {
            TotalCount = Query.TotalRecordCount;
            PageStatusString = (Limit + StartIndex) < TotalCount ? $"{StartIndex + 1} - {StartIndex + Limit} of {TotalCount}" : $"{StartIndex + 1} - {TotalCount} of {TotalCount}";
            NextButtonIsEnabled = (Limit + StartIndex) < TotalCount;
            BackButtonIsEnabled = StartIndex > 0;
        }
    }
}
