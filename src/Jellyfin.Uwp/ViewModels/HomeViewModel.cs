using CommonHelpers.Common;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {

        private bool hasLiveTvRecommendations;
        private bool hasContinueWatchingTvShows;
        private bool hasContinueListeningMusic;
        private bool hasNextUp;
        
        public bool HasLiveTvRecommendations { get => hasLiveTvRecommendations; set => SetProperty(ref hasLiveTvRecommendations, value); }
        public bool HasContinueWatchingTvShows { get => hasContinueWatchingTvShows; set => SetProperty(ref hasContinueWatchingTvShows, value); }
        public bool HasContinueListeningMusic { get => hasContinueListeningMusic; set => SetProperty(ref hasContinueListeningMusic, value); }
        public bool HasNextUp { get => hasNextUp; set => SetProperty(ref hasNextUp, value); }

        public ObservableCollection<MediaDataItem> LiveTvRecommendationCollection { get; set; }
        public ObservableCollection<MediaDataItem> ContinueWatchingTvShowsCollection { get; set; }
        public ObservableCollection<MediaDataItem> ContinueListeningMusicCollection { get; set; }
        public ObservableCollection<MediaDataItem> NextUpCollection { get; set; }
        public ObservableCollection<MediaDataItemCollection> LatestMedia { get; set; }
        public ObservableCollection<MediaDataItem> Libraries { get; set; }

        public HomeViewModel()
        {
            HasLiveTvRecommendations = false;
            HasContinueWatchingTvShows = false;
            HasContinueListeningMusic = false;
            HasNextUp = false;
            LatestMedia = new ObservableCollection<MediaDataItemCollection>();
            Libraries = new ObservableCollection<MediaDataItem>();
        }

        //Load Home Page Content
        public async Task PageReadyAsync()
        {
            IsBusy = true;
            IsBusyMessage = "Loading Page Content..";

            LoadLibraries();

            await LoadLiveTvRecommendations();

            await LoadContinueWatchingTvShows();

            await LoadContinueListeningMusic();

            await LoadNextUp();

            await LoadLatestMedia();

            IsBusy = false;
            IsBusyMessage = "";
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

        private async Task LoadLiveTvRecommendations()
        {
            JellyfinClientServices.Current.LiveTvClient = new LiveTvClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            BaseItemDtoQueryResult LiveTvRecomendations = await JellyfinClientServices.Current.LiveTvClient.GetRecommendedProgramsAsync(
                userId: App.Current.AppUser.User.Id,
                isAiring: true,
                limit:1,
                imageTypeLimit:1,
                enableImageTypes: new ImageType[]
                {
                    ImageType.Primary,
                    ImageType.Thumb,
                    ImageType.Backdrop
                },
                fields: new ItemFields[]
                {
                    ItemFields.ChannelInfo,
                    ItemFields.PrimaryImageAspectRatio
                });

            if (LiveTvRecomendations.TotalRecordCount > 0)
            {
                HasLiveTvRecommendations = true;
                LiveTvRecommendationCollection = new ObservableCollection<MediaDataItem>();

                foreach (BaseItemDto item in LiveTvRecomendations.Items)
                {
                    LiveTvRecommendationCollection.Add(new MediaDataItem
                    {
                        BaseItem = item,
                        Height = 486,
                        Width = 324,
                    });
                }
            }
        }

        private async Task LoadContinueWatchingTvShows()
        {
            BaseItemDtoQueryResult TvShowsContinueWatching = await JellyfinClientServices.Current.ItemsClient.GetResumeItemsAsync(
                App.Current.AppUser.User.Id,
                limit: 12,
                fields: new ItemFields[]
                {
                    ItemFields.PrimaryImageAspectRatio,
                    ItemFields.BasicSyncInfo
                },
                enableImageTypes: new ImageType[]
                {
                    ImageType.Primary,
                    ImageType.Backdrop,
                    ImageType.Thumb
                },
                mediaTypes: new string[]
                {
                    "Video"
                });

            if (TvShowsContinueWatching.TotalRecordCount > 0)
            {
                HasContinueWatchingTvShows = true;
                ContinueWatchingTvShowsCollection = new ObservableCollection<MediaDataItem>();

                foreach (BaseItemDto item in TvShowsContinueWatching.Items)
                {
                    ContinueWatchingTvShowsCollection.Add(new MediaDataItem
                    {
                        BaseItem = item,
                        Height = 324,
                        Width = 486,
                    });
                }
            }
        }

        private async Task LoadContinueListeningMusic()
        {
            BaseItemDtoQueryResult MusicContinueListening = await JellyfinClientServices.Current.ItemsClient.GetResumeItemsAsync(
                App.Current.AppUser.User.Id,
                limit: 12,
                fields: new ItemFields[]
                {
                    ItemFields.PrimaryImageAspectRatio,
                    ItemFields.BasicSyncInfo
                },
                enableImageTypes: new ImageType[]
                {
                    ImageType.Primary,
                    ImageType.Backdrop,
                    ImageType.Thumb
                },
                mediaTypes: new string[]
                {
                    "Audio"
                });

            if (MusicContinueListening.TotalRecordCount > 0)
            {
                HasContinueListeningMusic = true;
                ContinueListeningMusicCollection = new ObservableCollection<MediaDataItem>();

                foreach (BaseItemDto item in MusicContinueListening.Items)
                {
                    ContinueWatchingTvShowsCollection.Add(new MediaDataItem
                    {
                        BaseItem = item,
                        Height = 300,
                        Width = 300,
                    });
                }
            }
        }

        private async Task LoadNextUp()
        {
            BaseItemDtoQueryResult NextUpEpisodes = await JellyfinClientServices.Current.TvShowsClient.GetNextUpAsync(
            userId: App.Current.AppUser.User.Id,
            limit: 24,
            imageTypeLimit: 1,
            enableImageTypes: new ImageType[]
            {
                ImageType.Primary,
                ImageType.Backdrop,
                ImageType.Thumb
            },
            enableTotalRecordCount: true);

            if (NextUpEpisodes.TotalRecordCount > 0)
            {
                HasNextUp = true;
                NextUpCollection = new ObservableCollection<MediaDataItem>();

                foreach (BaseItemDto item in NextUpEpisodes.Items)
                {
                    NextUpCollection.Add(new MediaDataItem
                    {
                        BaseItem = item,
                        Height = 324,
                        Width = 486,
                    });
                }
            }
        }

        private async Task LoadLatestMedia()
        {


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
                        default:
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

                        // TODO: Add Title and Subtitle for MediaItemUserControl
                        ltmiList.Add(new MediaDataItem
                        {
                            BaseItem = LatestMediaItem,
                            Height = height,
                            Width = width,
                            UpdateInterval = new TimeSpan(0, 0, new Random().Next(5, 35))
                        });
                    }

                    LatestMedia.Add(new MediaDataItemCollection
                    {
                        Name = $"Latest {item.BaseItem.Name}",
                        LatestItems = ltmiList
                    });
                }
            }
        }
    }
}
