using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ObservableCollection<LatestMediaModel> LatestMedia { get; set; }
        public ObservableCollection<LibrariesModel> Libraries { get; set; }

        public HomeViewModel()
        {
            LatestMedia = new ObservableCollection<LatestMediaModel>();
            Libraries = new ObservableCollection<LibrariesModel>();
        }

        //Load Home Page Content
        public async Task PageReadyAsync()
        {
            LoadLibraries();

            await LoadLatestMedia();
        }

        private void LoadLibraries()
        {
            foreach (var item in App.Current.UserViews.Items)
            {
                Libraries.Add(new LibrariesModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    ImageSrc = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{item.DisplayPreferencesId}/Images/Primary")),
                    Type = item.CollectionType,
                    UpdateInterval = new TimeSpan(0, 0, new Random().Next(2, 9))
                });
            }
        }

        private async Task LoadLatestMedia()
        {
            foreach (var item in this.Libraries)
            {
                IReadOnlyList<BaseItemDto> LibraryLatestMedia = await UserLibraryClientService.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.Id, item.Id);
                if (item.Type != "boxsets")
                {
                    List<LatestMediaItemModel> ltmiList = new List<LatestMediaItemModel>();

                    foreach (var LatestMediaItem in LibraryLatestMedia)
                    {
                        ltmiList.Add(new LatestMediaItemModel
                        {
                            Id = LatestMediaItem.Id,
                            Name = LatestMediaItem.Name,
                            ImageSrc = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{LatestMediaItem.Id}/Images/Primary"))
                        });
                    }

                    LatestMedia.Add(new LatestMediaModel
                    {
                        Name = $"Latest {item.Name}",
                        Id = item.Id,
                        LatestItems = ltmiList
                    });
                }
            }
        }
    }

    public class LatestMediaItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ImageSource ImageSrc { get; set; }
    }

    public class LibrariesModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ImageSource ImageSrc { get; set; }
        public string Type { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }

    public class LatestMediaModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<LatestMediaItemModel> LatestItems { get; set; }
    }
}
