using CommonHelpers.Common;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ObservableCollection<LatestMedia> LatestMedia { get; set; }
        public ObservableCollection<MediaDataItem> Libraries { get; set; }

        public HomeViewModel()
        {
            LatestMedia = new ObservableCollection<LatestMedia>();
            Libraries = new ObservableCollection<MediaDataItem>();
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
                Libraries.Add(new MediaDataItem
                {
                    BaseItem = item,
                    UpdateInterval = new TimeSpan(0, 0, new Random().Next(5, 15)),
                    Height = 405,
                    Width = 720
                });
            }
        }

        private async Task LoadLatestMedia()
        {
            IsBusy = true;
            IsBusyMessage = "Loading Page Content..";

            foreach (MediaDataItem item in Libraries)
            {
                if (item.BaseItem.CollectionType != "boxsets")
                {

                    IReadOnlyList<BaseItemDto> LatestMediaItems;

                    switch (item.BaseItem.CollectionType)
                    {
                        case "tvshows":
                            LatestMediaItems =
                                await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(
                                    App.Current.AppUser.User.Id,
                                    parentId: item.BaseItem.Id,
                                    fields: new ItemFields[] {
                                        ItemFields.PrimaryImageAspectRatio,
                                        ItemFields.SeriesPrimaryImage,
                                        ItemFields.BasicSyncInfo,
                                        ItemFields.Path
                                    },
                                    includeItemTypes: new string[] { "Episode" },
                                    imageTypeLimit: 1,
                                    isPlayed: false,
                                    enableImages: true,
                                    enableImageTypes: new ImageType[] {
                                        ImageType.Primary,
                                        ImageType.Backdrop,
                                        ImageType.Thumb
                                    },
                                    limit: 20
                                );
                            break;
                        case "movies":
                            LatestMediaItems =
                                await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(
                                    App.Current.AppUser.User.Id,
                                    parentId: item.BaseItem.Id,
                                    fields: new ItemFields[] {
                                        ItemFields.PrimaryImageAspectRatio,
                                        ItemFields.BasicSyncInfo,
                                        ItemFields.Path
                                    },
                                    imageTypeLimit: 1,
                                    includeItemTypes: new string[] { "Movie" },
                                    isPlayed: false,
                                    enableImages: true,
                                    enableImageTypes: new ImageType[] { ImageType.Primary },
                                    limit: 20
                                );
                            break;
                        case "homevideos":
                            LatestMediaItems =
                                await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.User.Id,
                                    parentId: item.BaseItem.Id,
                                    fields: new ItemFields[] {
                                        ItemFields.PrimaryImageAspectRatio,
                                        ItemFields.BasicSyncInfo,
                                        ItemFields.Path
                                    },
                                    imageTypeLimit: 1,
                                    enableImages: true,
                                    enableImageTypes: new ImageType[] { ImageType.Primary },
                                    limit: 20
                                );
                            break;
                        case "music":
                            LatestMediaItems =
                                await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(
                                    App.Current.AppUser.User.Id,
                                    parentId: item.BaseItem.Id,
                                    fields: new ItemFields[] {
                                        ItemFields.PrimaryImageAspectRatio,
                                        ItemFields.BasicSyncInfo,
                                        ItemFields.Path
                                    },
                                    imageTypeLimit: 1,
                                    enableImages: true,
                                    enableImageTypes: new ImageType[] { ImageType.Primary },
                                    limit: 20
                                );
                            break;
                        case "books":
                            LatestMediaItems =
                                await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.User.Id,
                                    parentId: item.BaseItem.Id,
                                    fields: new ItemFields[] {
                                        ItemFields.PrimaryImageAspectRatio,
                                        ItemFields.BasicSyncInfo,
                                        ItemFields.Path
                                    },
                                    imageTypeLimit: 1,
                                    enableImages: true,
                                    enableImageTypes: new ImageType[] { ImageType.Primary },
                                    limit: 20
                                );
                            break;
                        default:
                            LatestMediaItems =
                                await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.User.Id,
                                    parentId: item.BaseItem.Id,
                                    fields: new ItemFields[] {
                                        ItemFields.PrimaryImageAspectRatio,
                                        ItemFields.BasicSyncInfo,
                                        ItemFields.Path
                                    },
                                    imageTypeLimit: 1,
                                    enableImages: true,
                                    enableImageTypes: new ImageType[] { ImageType.Primary },
                                    limit: 20
                                );
                            break;
                    }


                    List<MediaDataItem> ltmiList = new List<MediaDataItem>();

                    foreach (BaseItemDto LatestMediaItem in LatestMediaItems)
                    {
                        int height, width;

                        if (item.BaseItem.CollectionType == "tvshows" ||
                            item.BaseItem.CollectionType == "movies" ||
                            item.BaseItem.CollectionType == "homevideos")
                        {
                            height = 486;
                            width = 324;
                        }
                        else
                        {
                            height = 300;
                            width = 300;
                        }

                        ltmiList.Add(new MediaDataItem
                        {
                            BaseItem = LatestMediaItem,
                            Height = height,
                            Width = width,
                            UpdateInterval = new TimeSpan(0, 0, new Random().Next(5, 35))
                        });
                    }

                    LatestMedia.Add(new LatestMedia
                    {
                        Name = $"Latest {item.BaseItem.Name}",
                        LatestItems = ltmiList
                    });
                }
            }

            IsBusy = false;
            IsBusyMessage = "";
        }
    }
}
