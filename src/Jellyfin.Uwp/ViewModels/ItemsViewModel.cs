using CommonHelpers.Common;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class ItemsViewModel : ViewModelBase
    {
        public ObservableCollection<MediaDataItem> GridItems { get; set; }

        public ItemsViewModel()
        {
            GridItems = new ObservableCollection<MediaDataItem>();
        }

        public async Task PageReadyAsync(Guid LibraryId)
        {
            await LoadLibraryItemsAsync(LibraryId);
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
                    GridItems.Add(new MediaDataItem
                    {
                        BaseItem = item,
                        ImageSource = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{item.Id}/Images/Primary"))
                    });
                }
            }
        }
    }
}
