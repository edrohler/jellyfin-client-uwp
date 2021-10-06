using CommonHelpers.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;

namespace Jellyfin.ViewModels
{
    public class MediaPlayerViewModel : ViewModelBase
    {
        public MediaInfoClient MediaInfoClient;
        public PlaystateClient PlayStateClient;
        public VideosClient VideoClient;
        public TvShowsClient TvShowClient;
        public ArtistsClient ArtistsClient;

        public BaseItemDto BaseItem;
        public ChapterInfo BaseItemChapterInfo;
        public MediaSourceInfo BaseItemMediaSourceInfo;

        public PlaybackStartInfo PlaybackStartInfo;
        public PlaybackStopInfo PlaybackStopInfo;
        public PlaybackInfoResponse PlaybackInfo;

        public Uri StreamUri { get; set; }

        public MediaPlayerViewModel()
        {
            MediaInfoClient = new MediaInfoClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            PlayStateClient = new PlaystateClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            VideoClient = new VideosClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
        }

        public async Task PageReadyAsync(Guid Id)
        {
            BaseItem = await JellyfinClientServices.Current.UserLibraryClient.GetItemAsync(App.Current.AppUser.User.Id, Id);

            if (string.IsNullOrEmpty(BaseItem.MediaType))
            {
                switch (BaseItem.Type)
                {
                    case "Series":
                        ////Find first unwatched episode of the series and set stream uri
                        /// Create the TvShowsClient
                        TvShowClient = new TvShowsClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
                        // Query the Seasons
                        BaseItemDtoQueryResult seasons = await TvShowClient.GetSeasonsAsync(BaseItem.Id);
                        // Get the SeriesId
                        Guid seriesId = (Guid)seasons.Items.First().SeriesId;
                        // Get the NextUp (First Unwatched item)
                        BaseItemDtoQueryResult nextup = await TvShowClient.GetNextUpAsync(userId: App.Current.AppUser.User.Id, seriesId: seriesId.ToString());
                        BaseItem = nextup.Items.First();
                        break;
                    case "MusicAlbum":
                        //// Get Album songs from Id on Homepage
                        // Get album
                        BaseItemDtoQueryResult album = await JellyfinClientServices.Current.ItemsClient.GetItemsAsync(userId:App.Current.AppUser.User.Id, parentId: BaseItem.Id);
                        // Get first song
                        BaseItemDto firstsong = album.Items.First();
                        BaseItem = await JellyfinClientServices.Current.UserLibraryClient.GetItemAsync(App.Current.AppUser.User.Id, firstsong.Id);
                        break;
                }
            }

            switch (BaseItem.MediaType)
            {
                case "Video":
                    StreamUri = new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Videos/{BaseItem.Id}/stream?static=true");
                    break;
                case "Audio":
                    StreamUri = new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Audio/{BaseItem.Id}/stream?static=true");
                    break;
                default:
                    // Something Happened Log it and go back to Shell Page
                    ExceptionLogger.LogException(new Exception($"Navigation to Media {BaseItem.Name} Failed")
                    {
                        Source = $"{AppDomain.CurrentDomain.FriendlyName} - {GetType().Name} - {MethodBase.GetCurrentMethod()}"
                    });
                    App.Current.RootFrame.Navigate(typeof(ShellPage)); ;
                    break;
            }

            ////var streamUrl = $"{App.Current.SdkClientSettings.BaseUrl}/Videos/{Id}";

            //// Report back to Server that Playback has Started.

            //SessionId = Guid.NewGuid();

            //PlaybackInfo = await MediaInfoClient.GetPlaybackInfoAsync(Id, App.Current.AppUser.Id);

            //foreach (MediaSourceInfo mediaSourceInfo in PlaybackInfo.MediaSources)
            //{
            //    BaseItemMediaSourceInfo = mediaSourceInfo;

            //    foreach (MediaStream mediaStream in BaseItem.MediaStreams)
            //    {
            //        switch (mediaStream.Type)
            //        {
            //            case MediaStreamType.Audio:
            //                break;
            //            case MediaStreamType.Video:
            //                break;
            //            case MediaStreamType.Subtitle:
            //                break;
            //            case MediaStreamType.EmbeddedImage:
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //}

            //PlaybackStartInfo = new PlaybackStartInfo
            //{
            //    ItemId = Id,
            //    Item = BaseItem,
            //    CanSeek = true,
            //    SessionId = SessionId.ToString()
            //};

            //await PlayStateClient.ReportPlaybackStartAsync(PlaybackStartInfo);


            Console.WriteLine();
        }
    }
}
