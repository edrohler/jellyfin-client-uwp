using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Helpers;
using Jellyfin.Models;
using Jellyfin.Models.Enums;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private string pageStatusString;
        private bool backButtonIsEnabled;
        private bool nextButtonIsEnabled;
        private BaseItemDtoQueryResult Query;
        private string[] sortBy;
        private SortOrder[] sortOrder;

        public int TotalCount { get => totalCount; set => SetProperty(ref totalCount, value); }
        public int StartIndex { get => startIndex; set => SetProperty(ref startIndex, value); }
        public int Limit { get => limit; set => SetProperty(ref limit, value); }
        public bool IsPageable { get => isPageable; set => SetProperty(ref isPageable, value); }
        public string PageStatusString { get => pageStatusString; set => SetProperty(ref pageStatusString, value); }
        public bool BackButtonIsEnabled { get => backButtonIsEnabled; set => SetProperty(ref backButtonIsEnabled, value); }
        public bool NextButtonIsEnabled { get => nextButtonIsEnabled; set => SetProperty(ref nextButtonIsEnabled, value); }
        public string[] SortBy { get => sortBy; set => SetProperty(ref sortBy, value); }
        public SortOrder[] SortOrder { get => sortOrder; set => SetProperty(ref sortOrder, value); }

        public ObservableCollection<MediaDataItem> GridItems { get; set; }
        public ObservableCollection<SortDataItem> SortByCollection { get; set; }
        public ObservableCollection<SortDataItem> SortOrderCollection { get; set; }
        public DelegateCommand NextPageCommand { get; set; }
        public DelegateCommand PrevPageCommand { get; set; }
        public DelegateCommand SortCommand { get; set; }
        public DelegateCommand FilterCommand { get; set; }

        public BaseItemDto UserView;

        public ItemsViewModel()
        {
            StartIndex = 0;
            Limit = 100;
            GridItems = new ObservableCollection<MediaDataItem>();
            SortByCollection = new ObservableCollection<SortDataItem>();
            SortOrderCollection = new ObservableCollection<SortDataItem>();
            NextPageCommand = new DelegateCommand(async () => await NextPageAsync());
            PrevPageCommand = new DelegateCommand(async () => await PrevPageAsync());
        }

        public async Task PageReadyAsync(Guid LibraryId)
        {
            // Get UserView
            UserView = App.Current.UserViews.Items.FirstOrDefault(x => x.Id == LibraryId);

            // Instantiate SortOrderCollection
            foreach (object item in Enum.GetValues(typeof(Sdk.SortOrder)))
            {
                SortOrderCollection.Add(new SortDataItem
                {
                    DisplayName = item.ToString(),
                    Value = item.ToString()
                });
            }

            // Set SortByCollection
            switch (UserView.CollectionType)
            {
                case "movies":
                    // Instantiate SortByCollection
                    foreach (object item in Enum.GetValues(typeof(MoviesSortBy)))
                    {
                        SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    SortOrder = new SortOrder[] { Sdk.SortOrder.Descending };
                    SortBy = new string[] { "DateCreated", "SortName", "ProductionYear" };

                    break;
                case "tvshows":
                    foreach (object item in Enum.GetValues(typeof(TvShowsSortBy)))
                    {
                        SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    SortOrder = new SortOrder[] { Sdk.SortOrder.Ascending };
                    SortBy = new string[] { "SortName" };

                    break;
                case "music":
                    foreach (object item in Enum.GetValues(typeof(MusicSortBy)))
                    {
                        SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    SortOrder = new SortOrder[] { Sdk.SortOrder.Ascending };
                    SortBy = new string[] { "SortName" };

                    break;
                default:
                    foreach (object item in Enum.GetValues(typeof(FolderSortBy)))
                    {
                        SortByCollection.Add(new SortDataItem
                        {
                            DisplayName = item.GetType()
                                .GetField(item.ToString())
                                .GetCustomAttribute<DisplayNameAttribute>()
                                .DisplayName,
                            Value = item.ToString()
                        });
                    }

                    SortOrder = new SortOrder[] { Sdk.SortOrder.Ascending };
                    SortBy = new string[] { "IsFolder", "SortName" };

                    break;
            }

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
                        sortBy: SortBy,
                        sortOrder: SortOrder,
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
                        parentId: UserView.Id);
                    UpdatePaging();
                    IsPageable = true;
                    break;
                case "tvshows":
                    // Get TV Shows Library Items
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: SortBy,
                        sortOrder: SortOrder,
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
                        parentId: UserView.Id);
                    UpdatePaging();
                    IsPageable = true;
                    break;
                case "movies":
                    // Get Movies Library Items
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: SortBy,
                        sortOrder: SortOrder,
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
                        parentId: UserView.Id);
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
                        sortBy: SortBy,
                        sortOrder: SortOrder);
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

        private void UpdatePaging()
        {
            TotalCount = Query.TotalRecordCount;
            PageStatusString = (Limit + StartIndex) < TotalCount ? $"{StartIndex + 1} - {StartIndex + Limit} of {TotalCount}" : $"{StartIndex + 1} - {TotalCount} of {TotalCount}";
            NextButtonIsEnabled = (Limit + StartIndex) < TotalCount;
            BackButtonIsEnabled = StartIndex > 0;
        }
    }
}
