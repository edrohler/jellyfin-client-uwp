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
        private bool isHd;
        private bool is4K;
        private bool is3D;
        private bool hasSubtitles;
        private bool hasTrailer;
        private bool hasSpecialFeature;
        private bool hasThemeSong;
        private bool hasThemeVideo;
        private string pageStatusString;
        private BaseItemDtoQueryResult query;
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
        public bool IsVideoTypesFilterVisible { get => isVideoTypesFilterVisible; set => SetProperty(ref isVideoTypesFilterVisible, value); }
        public bool IsYearsFilterVisible { get => isYearsFilterVisible; set => SetProperty(ref isYearsFilterVisible, value); }
        public bool IsHD { get => isHd; set => SetProperty(ref isHd, value); }
        public bool Is4K { get => is4K; set => SetProperty(ref is4K, value); }
        public bool Is3D { get => is3D; set => SetProperty(ref is3D, value); }
        public bool HasSubtitles { get => hasSubtitles; set => SetProperty(ref hasSubtitles, value); }
        public bool HasTrailer { get => hasTrailer; set => SetProperty(ref hasTrailer, value); }
        public bool HasSpecialFeature { get => hasSpecialFeature; set => SetProperty(ref hasSpecialFeature, value); }
        public bool HasThemeSong { get => hasThemeSong; set => SetProperty(ref hasThemeSong, value); }
        public bool HasThemeVideo { get => hasThemeVideo; set => SetProperty(ref hasThemeVideo, value); }
        public string PageStatusString { get => pageStatusString; set => SetProperty(ref pageStatusString, value); }
        public BaseItemDto UserView { get => userView; set => SetProperty(ref userView, value); }
        public BaseItemDtoQueryResult Query { get => query; set => SetProperty(ref query, value); }
        public ObservableCollection<MediaDataItem> GridItems { get; set; }
        public ObservableCollection<SortFilterDataItem> SortByCollection { get; set; }
        public ObservableCollection<string> SelectedSortBy { get; set; }
        public ObservableCollection<SortFilterDataItem> SortOrderCollection { get; set; }
        public ObservableCollection<SortOrder> SelectedSortOrder { get; set; }
        public ObservableCollection<SortFilterDataItem> FilterCollection { get; set; }
        public ObservableCollection<ItemFilter> SelectedFilters { get; set; }
        public ObservableCollection<SortFilterDataItem> FeaturesCollection { get; set; }
        public ObservableCollection<SortFilterDataItem> SeriesStatusCollection { get; set; }
        public ObservableCollection<SeriesStatus> SelectedSeriesStatus { get; set; }
        public ObservableCollection<SortFilterDataItem> GenresCollection { get; set; }
        public ObservableCollection<string> SelectedGenres { get; set; }
        public ObservableCollection<SortFilterDataItem> ParentalRatingCollection { get; set; }
        public ObservableCollection<string> SelectedParentalRatings { get; set; }
        public ObservableCollection<SortFilterDataItem> TagsCollection { get; set; }
        public ObservableCollection<string> SelectedTags { get; set; }
        public ObservableCollection<SortFilterDataItem> VideoTypesCollection { get; set; }
        public ObservableCollection<VideoType> SelectedVideoTypes { get; set; }
        public ObservableCollection<SortFilterDataItem> YearsCollection { get; set; }
        public ObservableCollection<int> SelectedYears { get; set; }
        public ObservableCollection<string> IncludeItemTypesCollection { get; set; }
        public ObservableCollection<ItemFields> FieldsCollection { get; set; }
        public ObservableCollection<ImageType> EnableImageTypesCollection { get; set; }
        public DelegateCommand NextPageCommand { get; set; }
        public DelegateCommand PrevPageCommand { get; set; }

        public ItemsViewModel()
        {
            StartIndex = 0;
            Limit = 100;
            GridItems = new ObservableCollection<MediaDataItem>();
            SortByCollection = new ObservableCollection<SortFilterDataItem>();
            SortOrderCollection = new ObservableCollection<SortFilterDataItem>();
            FilterCollection = new ObservableCollection<SortFilterDataItem>();
            NextPageCommand = new DelegateCommand(async () => await NextPageAsync());
            PrevPageCommand = new DelegateCommand(async () => await PrevPageAsync());

            IsFeaturesFilterVisible = false;
            IsGenresFilterVisible = false;
            IsParentalRatingsFilterVisible = false;
            IsStatusFilterVisible = false;
            IsTagsFilterVisible = false;
            IsVideoTypesFilterVisible = false;
            IsYearsFilterVisible = false;
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
            IsBusyMessage = "Loading Library Items..";

            if (GridItems.Count > 0)
            {
                GridItems.Clear();
            }

            switch (UserView.CollectionType)
            {
                case "music":
                    // Gets Music Library Items
                    //Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                    //    App.Current.AppUser.User.Id,
                    //    sortBy: SelectedSortBy.ToArray(),
                    //    sortOrder: SelectedSortOrder.ToArray(),
                    //    includeItemTypes: IncludeItemTypesCollection.ToArray(),
                    //    recursive: true,
                    //    fields: FieldsCollection.ToArray(),
                    //    imageTypeLimit: 1,
                    //    enableImageTypes: EnableImageTypesCollection.ToArray(),
                    //    startIndex: StartIndex,
                    //    limit: Limit,
                    //    parentId: UserView.Id,
                    //    filters: SelectedFilters.ToArray());
                    UpdatePaging();
                    IsPageable = true;

                    break;
                case "tvshows":
                    // Get TV Shows Library Items
                    //Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                    //    App.Current.AppUser.User.Id,
                    //    sortBy: SelectedSortBy.ToArray(),
                    //    sortOrder: SelectedSortOrder.ToArray(),
                    //    includeItemTypes: IncludeItemTypesCollection.ToArray(),
                    //    recursive: true,
                    //    fields: FieldsCollection.ToArray(),
                    //    imageTypeLimit: 1,
                    //    enableImageTypes: EnableImageTypesCollection.ToArray(),
                    //    startIndex: StartIndex,
                    //    limit: Limit,
                    //    parentId: UserView.Id,
                    //    filters: SelectedFilters.ToArray());
                    UpdatePaging();
                    IsPageable = true;

                    break;
                case "movies":
                    // Get Movies Library Items
                    //Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                    //    App.Current.AppUser.User.Id,
                    //    sortBy: SelectedSortBy.ToArray(),
                    //    sortOrder: SelectedSortOrder.ToArray(),
                    //    includeItemTypes: IncludeItemTypesCollection.ToArray(),
                    //    recursive: true,
                    //    fields: FieldsCollection.ToArray(),
                    //    imageTypeLimit: 1,
                    //    enableImageTypes: EnableImageTypesCollection.ToArray(),
                    //    startIndex: StartIndex,
                    //    limit: Limit,
                    //    parentId: UserView.Id,
                    //    filters: SelectedFilters.ToArray(),
                    //    officialRatings: SelectedParentalRatings.ToArray(), 
                    //    tags: SelectedTags.ToArray(),
                    //    years: SelectedYears.ToArray());
                    UpdatePaging();
                    IsPageable = true;

                    break;
                default:
                    // Get Audiobooks, Photos/Home Videos and Collections Items
                    //Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                    //    App.Current.AppUser.User.Id,
                    //    fields: FieldsCollection.ToArray(),
                    //    imageTypeLimit: 1,
                    //    parentId: UserView.Id,
                    //    sortBy: SelectedSortBy.ToArray(),
                    //    sortOrder: SelectedSortOrder.ToArray(),
                    //    filters: SelectedFilters.ToArray());
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

                    //try
                    //{
                    //    FileResponse fr = await JellyfinClientServices.Current.ImageClient.GetItemImageAsync(item.Id, ImageType.Primary);

                    //    using (Stream stream = fr.Stream)
                    //    {
                    //        using (MemoryStream ms = new MemoryStream())
                    //        {
                    //            await stream.CopyToAsync(ms);
                    //            ms.Position = 0;

                    //            await img.SetSourceAsync(ms.AsRandomAccessStream());
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    // TODO Load default image
                    //    ExceptionLogger.LogException(ex);
                    //}

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

        //private ItemFilter[] GetItemFilters()
        //{
        //    IEnumerable<SortFilterDataItem> SelectedFilters = FilterCollection.Where(i => i.IsSelected);

        //    ItemFilter[] filters = new ItemFilter[SelectedFilters.Count()];

        //    for (int i = 0; i < SelectedFilters.Count(); i++)
        //    {
        //        switch (SelectedFilters.ElementAt(i).Value)
        //        {
        //            case "IsUnPlayed":
        //                filters[i] = ItemFilter.IsUnplayed;
        //                break;
        //            case "IsPlayed":
        //                filters[i] = ItemFilter.IsPlayed;
        //                break;
        //            case "IsFavorite":
        //                filters[i] = ItemFilter.IsFavorite;
        //                break;
        //            case "IsResumable":
        //                filters[i] = ItemFilter.IsResumable;
        //                break;
        //            case "Likes":
        //                filters[i] = ItemFilter.Likes;
        //                break;
        //            case "Dislikes":
        //                filters[i] = ItemFilter.Dislikes;
        //                break;
        //        }
        //    }

        //    return filters;
        //}

        private void UpdatePaging()
        {
            TotalCount = Query.TotalRecordCount;
            PageStatusString = (Limit + StartIndex) < TotalCount ? $"{StartIndex + 1} - {StartIndex + Limit} of {TotalCount}" : $"{StartIndex + 1} - {TotalCount} of {TotalCount}";
            NextButtonIsEnabled = (Limit + StartIndex) < TotalCount;
            BackButtonIsEnabled = StartIndex > 0;
        }
    }
}
