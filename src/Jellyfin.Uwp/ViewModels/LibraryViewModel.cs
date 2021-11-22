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
        private string libraryTitle;
        private bool isPaneVisible;
        private BaseItemDto userView;

        public string LibraryTitle { get => libraryTitle; set => SetProperty(ref libraryTitle, value); }
        public bool IsPaneVisible { get => isPaneVisible; set => SetProperty(ref isPaneVisible, value); }
        public BaseItemDto UserView { get => userView; set => SetProperty(ref userView, value); }

        public ObservableCollection<MenuDataItem> LibraryPageMenuItems { get; set; }

        public LibraryViewModel()
        {
            LibraryPageMenuItems = new ObservableCollection<MenuDataItem>();
        }

        public void PageReady(Guid LibraryId)
        {
            UserView = App.Current.UserViews.Items.FirstOrDefault(i => i.Id == LibraryId);
            LoadMenuItems(UserView.CollectionType);
        }

        private void LoadMenuItems(string collectionType)
        {
            switch (collectionType)
            {
                case "tvshows":
                    LibraryTitle = UserView.Name;
                    string[] tvshowsMenuItems = { "Shows", "Suggestions", "Upcoming", "Genres", "Networks", "Episodes" };
                    foreach (string item in tvshowsMenuItems)
                    {
                        LibraryPageMenuItems.Add(new MenuDataItem
                        {
                            Name = item,
                            Id = UserView.Id
                        });
                    }
                    IsPaneVisible = true;
                    break;
                case "movies":
                    LibraryTitle = UserView.Name;
                    string[] moviesMenuItems = { "Movies", "Suggestions", "Trailers", "Favorites", "Collections", "Genres" };
                    foreach (string item in moviesMenuItems)
                    {
                        LibraryPageMenuItems.Add(new MenuDataItem
                        {
                            Name = item,
                            Id = UserView.Id
                        });
                    }
                    IsPaneVisible = true;
                    break;
                case "music":
                    LibraryTitle = UserView.Name;
                    string[] musicMenuItems = { "Albums", "Suggestions", "Album Artists", "Artists", "Playlists", "Songs", "Genres" };
                    foreach (string item in musicMenuItems)
                    {
                        LibraryPageMenuItems.Add(new MenuDataItem
                        {
                            Name = item,
                            Id = UserView.Id
                        });
                    }
                    IsPaneVisible = true;
                    break;
                default:
                    // No Menu Items for Audiobooks, Videos, Photos, Collections
                    LibraryTitle = UserView.Name;
                    IsPaneVisible = false;
                    // Play All, Shuffle, Sort, Filter
                    break;
            }
        }
    }
}
