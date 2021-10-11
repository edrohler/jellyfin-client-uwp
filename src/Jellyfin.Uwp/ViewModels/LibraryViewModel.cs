using CommonHelpers.Common;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;



namespace Jellyfin.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        public string LibraryTitle { get; set; }
        public bool IsPaneVisible { get; set; } = true;
        BaseItemDto BaseItem { get; set; }
        public ObservableCollection<MediaDataItem> LibraryItems { get; set; }
        public ObservableCollection<MenuDataItem> LibraryPageMenuItems { get; set; }

        public LibraryViewModel()
        {
            LibraryItems = new ObservableCollection<MediaDataItem>();
            LibraryPageMenuItems = new ObservableCollection<MenuDataItem>();
        }

        public async Task PageReadyAsync(Guid LibraryId)
        {
            BaseItem = App.Current.UserViews.Items.FirstOrDefault(i => i.Id == LibraryId);
            LoadMenuItems(BaseItem.CollectionType);
            await LoadLibraryItemsAsync(LibraryId);
        }

        private void LoadMenuItems(string collectionType)
        {
            switch (collectionType)
            {
                case "tvshows":
                    LibraryTitle = BaseItem.Name;
                    string[] tvshowsMenuItems = { "Shows", "Suggestions", "Upcoming", "Genres", "Networks", "Episodes" };
                    foreach (string item in tvshowsMenuItems)
                    {
                        LibraryPageMenuItems.Add(new MenuDataItem
                        {
                            Name = item,
                            Id = BaseItem.Id
                        });
                    }

                    break;
                case "movies":
                    LibraryTitle = BaseItem.Name;
                    string[] moviesMenuItems = { "Movies", "Suggestions", "Trailers", "Favorites", "Collections", "Genres" };
                    foreach (string item in moviesMenuItems)
                    {
                        LibraryPageMenuItems.Add(new MenuDataItem
                        {
                            Name = item,
                            Id = BaseItem.Id
                        });
                    }
                    break;
                case "music":
                    LibraryTitle = BaseItem.Name;
                    string[] musicMenuItems = { "Albums", "Suggestions", "Album Artists", "Artists", "Playlists", "Songs", "Genres" };
                    foreach (string item in musicMenuItems)
                    {
                        LibraryPageMenuItems.Add(new MenuDataItem
                        {
                            Name = item,
                            Id = BaseItem.Id
                        });
                    }
                    break;
                default:
                    // No Menu Items for Audiobooks, Videos, Photos, Collections
                    LibraryTitle = BaseItem.Name;
                    IsPaneVisible = false;
                    // Play All, Shuffle, Sort, Filter
                    break;
            }
        }

        public async Task LoadLibraryItemsAsync(Guid libId)
        {
            BaseItemDto UserView = App.Current.UserViews.Items.FirstOrDefault(x => x.Id == libId);
            BaseItemDtoQueryResult Query;

            switch (UserView.CollectionType)
            {
                case "music":
                    // Gets Music Library Items
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: new string[] { "SortName" },
                        sortOrder: new SortOrder[] { SortOrder.Ascending },
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
                        startIndex: 0,
                        limit: 100,
                        parentId: libId);
                    break;
                case "tvshows":
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: new string[] { "SortName" },
                        sortOrder: new SortOrder[] { SortOrder.Ascending },
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
                        startIndex: 0,
                        limit: 100,
                        parentId: libId);
                    break;
                case "movies":
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        sortBy: new string[] { "DateCreated", "SortName", "ProductionYear" },
                        sortOrder: new SortOrder[] { SortOrder.Descending },
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
                        startIndex: 0,
                        limit: 100,
                        parentId: libId);
                    break;
                default:
                    Query = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.SortName,
                            ItemFields.Path
                        },
                        imageTypeLimit: 1,
                        parentId: libId,
                        sortBy: new string[]
                        {
                            "IsFolder",
                            "SortName"
                        },
                        sortOrder: new SortOrder[]
                        {
                            SortOrder.Ascending
                        });
                    break;
            }


            if (Query != null)
            {
                // Creates GridView Collection
                foreach (BaseItemDto item in Query.Items)
                {
                    LibraryItems.Add(new MediaDataItem
                    {
                        BaseItem = item,
                        ImageSource = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{item.Id}/Images/Primary"))
                    });
                }
            }
        }
    }
}
