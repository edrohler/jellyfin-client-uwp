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
        public SessionClient SessionClient;
        public PlaystateClient PlayStateClient;
        public MoviesClient MovieClient;
        public VideosClient VideoClient;
        public TvShowsClient TvShowClient;


        public BaseItemDto BaseItem;
        public ChapterInfo BaseItemChapterInfo;
        public MediaSourceInfo BaseItemMediaSourceInfo;

        public PlaybackStartInfo PlaybackStartInfo;
        public PlaybackStopInfo PlaybackStopInfo;
        public PlaybackInfoResponse PlaybackInfo;
        public Guid SessionId;

        public MediaStreamSource MediaStreamSource {get;set;}
        public MediaSource MediaSource { get; set; }

        public byte[] StreamBuffer;

        public Uri StreamUri { get; set; }

        public MediaPlayerViewModel()
        {
            MediaInfoClient = new MediaInfoClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            SessionClient = new SessionClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            PlayStateClient = new PlaystateClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            VideoClient = new VideosClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            MovieClient = new MoviesClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
        }

        public async Task PageReadyAsync(Guid Id)
        {
            // For testing, stop all sessions.
            //await StopAllDeviceSessions(App.Current.SdkClientSettings.DeviceId.ToString());

            BaseItem = await JellyfinClientServices.Current.UserLibraryClient.GetItemAsync(App.Current.AppUser.Id, Id);

            if (string.IsNullOrEmpty(BaseItem.MediaType))
            {
                switch (BaseItem.Type)
                {
                    case "Series":
                        //// Find first unwatched episode of the series and set stream uri
                        TvShowClient = new TvShowsClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
                        var series = await TvShowClient.GetEpisodesAsync(BaseItem.Id);
                        Console.WriteLine();
                        break;
                    case "Album":
                        Console.WriteLine();
                        break;
                }
            }

            switch (BaseItem.MediaType)
            {
                case "Video":
                    StreamUri = new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Videos/{Id}/stream?static=true");
                    break;
                case "Audio":
                    StreamUri = new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Audio/{Id}/stream?static=true");
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

        public async Task StopAllDeviceSessions(string deviceId)
        {
            ICollection<SessionInfo> sessionsInfo = (ICollection<SessionInfo>)await SessionClient.GetSessionsAsync();

            SessionInfo session = sessionsInfo.FirstOrDefault(i => i.DeviceId == deviceId);

            if (session.NowPlayingItem != null)
            {
                foreach (SessionInfo item in sessionsInfo)
                {
                    PlaybackStopInfo = new PlaybackStopInfo
                    {
                        Item = session.NowPlayingItem,
                        ItemId = session.NowPlayingItem.Id,
                        SessionId = session.Id
                    };

                    await PlayStateClient.ReportPlaybackStoppedAsync(PlaybackStopInfo);
                }
            }
        }
    }
}
