using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommonHelpers.Common;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ObservableCollection<LatestMediaDataItems> LatestMedia { get; set; }
        public ObservableCollection<LibraryDataItems> Libraries { get; set; }

        public HomeViewModel()
        {
            LatestMedia = new ObservableCollection<LatestMediaDataItems>();
            Libraries = new ObservableCollection<LibraryDataItems>();
        }

        //Load Home Page Content
        public async Task PageReadyAsync()
        {
            LoadLibraries();

            await LoadLatestMedia();
        }

        private void LoadLibraries()
        {
            foreach (BaseItemDto item in App.Current.UserViews.Items)
            {
                Libraries.Add(new LibraryDataItems
                {
                    Id = item.Id,
                    Name = item.Name,
                    ImageSrc = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{item.DisplayPreferencesId}/Images/Primary")),
                    Type = item.CollectionType,
                    UpdateInterval = new TimeSpan(0, 0, new Random().Next(5, 15))
                });
            }
        }

        private async Task LoadLatestMedia()
        {
            foreach (LibraryDataItems item in this.Libraries)
            {
                ICollection<BaseItemDto> LibraryLatestMedia = (ICollection<BaseItemDto>)await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.Id, item.Id);
                if (item.Type != "boxsets")
                {
                    List<LatestMediaDataItem> ltmiList = new List<LatestMediaDataItem>();

                    foreach (BaseItemDto LatestMediaItem in LibraryLatestMedia)
                    {

                        int h, w;
                        switch (item.Type)
                        {
                            case "tvshows":
                                h = 486;
                                w = 324;
                                break;
                            case "movies":
                                h = 486;
                                w = 324;
                                break;
                            case "homevideos":
                                h = 486;
                                w = 324;
                                break;
                            default:
                                h = 300;
                                w = 300;
                                break;
                        }

                        ltmiList.Add(new LatestMediaDataItem
                        {
                            Id = LatestMediaItem.Id,
                            Name = LatestMediaItem.Name,
                            ImageSrc = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{LatestMediaItem.Id}/Images/Primary")),
                            Height = h,
                            Width = w,
                            UpdateInterval = new TimeSpan(0, 0, new Random().Next(5, 35))
                        });
                    }

                    LatestMedia.Add(new LatestMediaDataItems
                    {
                        Name = $"Latest {item.Name}",
                        Id = item.Id,
                        LatestItems = ltmiList
                    });
                }
            }
        }
    }
}
