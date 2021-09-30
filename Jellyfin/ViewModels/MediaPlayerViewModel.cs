using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            TvShowClient = new TvShowsClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
            MovieClient = new MoviesClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
        }

        public async Task PageReadyAsync(Guid Id)
        {
            // For testing, stop all sessions.
            //await StopAllDeviceSessions(App.Current.SdkClientSettings.DeviceId.ToString());

            BaseItem = await JellyfinClientServices.Current.UserLibraryClient.GetItemAsync(App.Current.AppUser.Id, Id);

            //FileResponse fr = await VideoClient.HeadVideoStreamAsync(BaseItem.Id);


            //using (MemoryStream ms = new MemoryStream())
            //{
            //    await fr.Stream.CopyToAsync(ms);
            //    ms.Position = 0;

            //    MediaSource.CreateFromStream(ms.AsRandomAccessStream(), "application/octet-stream");
            //}

            StreamUri = new Uri( $"{App.Current.SdkClientSettings.BaseUrl}/Videos/{Id}/Stream?static=true");

            //MediaSource.CreateFromUri(StreamUri);

            //switch (BaseItem.Type)
            //{
            //    case "Series":

            //        break;
            //    case "Movie":
                    
            //        break;
            //    case "AudioBook":

            //        break;
            //    case "MusicAlbum":

            //        break;
            //    case "Video":

            //        break;
            //    default:
            //        break;
            //}

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
