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
        BaseItemDto BaseItem { get; set; }
        public ObservableCollection<LibraryDataItem> LibraryItems { get; set; }
        public ObservableCollection<MenuDataItem> MenuItems { get; set; }

        public LibraryViewModel()
        {
            LibraryItems = new ObservableCollection<LibraryDataItem>();
            MenuItems = new ObservableCollection<MenuDataItem>();
        }

        public async Task PageReadyAsync(Guid LibraryId)
        {
            BaseItem = App.Current.UserViews.Items.Where(i => i.Id == LibraryId).FirstOrDefault();
            LoadMenuItems(LibraryId, BaseItem.CollectionType);
            await LoadLibraryItemsAsync(LibraryId);
        }

        private void LoadMenuItems(Guid libraryId, string collectionType)
        {
            switch (collectionType)
            {
                case "tvshows":
                    LibraryTitle = BaseItem.Name;
                    string[] tvshowsMenuItems = { "Shows", "Suggestions", "Upcoming", "Genres", "Networks", "Episodes" };
                    foreach (var item in tvshowsMenuItems)
                    {
                        MenuItems.Add(new MenuDataItem
                        {
                            Name = item
                        });
                    }
                    break;
                case "movies":
                    LibraryTitle = BaseItem.Name;
                    string[] moviesMenuItems = { "Movies", "Suggestions", "Trailers", "Favorites", "Collections", "Genres" };
                    foreach (var item in moviesMenuItems)
                    {
                        MenuItems.Add(new MenuDataItem
                        {
                            Name = item
                        });
                    }
                    break;
                case "music":
                    LibraryTitle = BaseItem.Name;
                    string[] musicMenuItems = { "Albums", "Suggestions", "Album Artists", "Artists", "Playlists", "Songs", "Genres" };
                    foreach (var item in musicMenuItems)
                    {
                        MenuItems.Add(new MenuDataItem
                        {
                            Name = item
                        });
                    }
                    break;
                default:
                    // No Menu Items for Audiobooks, Videos, Photos, Collections
                    LibraryTitle = BaseItem.Name;
                    // Play All, Shuffle, Sort, Filter
                    break;
            }
        }

        public async Task LoadLibraryItemsAsync(Guid libId)
        {
            // Gets Library Items
            BaseItemDtoQueryResult items = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(App.Current.AppUser.Id, parentId: libId);

            // Creates GridView Collection
            foreach (BaseItemDto item in items.Items)
            {
                LibraryItems.Add(new LibraryDataItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    ImageSrc = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{item.Id}/Images/Primary"))
                });
            }
        }
    }
}
