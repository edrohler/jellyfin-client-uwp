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
        public BaseItemDto BaseItem { get; set; }

        public ObservableCollection<MenuDataItem> LibraryPageMenuItems { get; set; }

        public LibraryViewModel()
        {
            LibraryPageMenuItems = new ObservableCollection<MenuDataItem>();

        }

        public void PageReady(Guid LibraryId)
        {
            BaseItem = App.Current.UserViews.Items.FirstOrDefault(i => i.Id == LibraryId);
            LoadMenuItems(BaseItem.CollectionType);
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
    }
}
