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
        public ObservableCollection<MenuDataItem> MenuItems { get; set; }

        public LibraryViewModel()
        {
            LibraryItems = new ObservableCollection<MediaDataItem>();
            MenuItems = new ObservableCollection<MenuDataItem>();
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
                        MenuItems.Add(new MenuDataItem
                        {
                            Name = item
                        });
                    }

                    break;
                case "movies":
                    LibraryTitle = BaseItem.Name;
                    string[] moviesMenuItems = { "Movies", "Suggestions", "Trailers", "Favorites", "Collections", "Genres" };
                    foreach (string item in moviesMenuItems)
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
                    foreach (string item in musicMenuItems)
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
                    IsPaneVisible = false;
                    // Play All, Shuffle, Sort, Filter
                    break;
            }
        }

        public async Task LoadLibraryItemsAsync(Guid libId)
        {
            // Gets Library Items
            BaseItemDtoQueryResult items = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(App.Current.AppUser.User.Id, parentId: libId);

            // Creates GridView Collection
            foreach (BaseItemDto item in items.Items)
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
