using Jellyfin.Sdk;

namespace Jellyfin.Services
{
    public class JellyfinClientServices
    {
        #region Singleton Members

        private static JellyfinClientServices _current;

        public static JellyfinClientServices Current => _current ?? (_current = new JellyfinClientServices());

        #endregion

        #region Instance Members

        public ActivityLogClient ActivityLogClient { get; set; }
        public ApiKeyClient ApiKeyClient { get; set; }
        public ArtistsClient ArtistsClient { get; set; }
        public AudioClient AudioClient { get; set; }
        public BrandingClient BrandingClient { get; set; }
        public ChannelsClient ChannelsClient { get; set; }
        public CollectionClient CollectionClient { get; set; }
        public ConfigurationClient ConfigurationClient { get; set; }
        public DashboardClient DashboardClient { get; set; }
        public DevicesClient DevicesClient { get; set; }
        public DisplayPreferencesClient DisplayPreferencesClient { get; set; }
        public DlnaClient DlnaClient { get; set; }
        public DlnaServerClient DlnaServerClient { get; set; }
        public DynamicHlsClient DynamicHlsClient { get; set; }
        public EnvironmentClient EnvironmentClient { get; set; }
        public FilterClient FilterClient { get; set; }
        public GenresClient GenresClient { get; set; }
        public HlsSegmentClient HlsSegmentClient { get; set; }
        public ImageByNameClient ImageByNameClient { get; set; }
        public ImageClient ImageClient { get; set; }
        public InstantMixClient InstantMixClient { get; set; }
        public ItemLookupClient ItemLookupClient { get; set; }
        public ItemRefreshClient ItemRefreshClient { get; set; }
        public ItemsClient ItemsClient { get; set; }
        public ItemUpdateClient ItemUpdateClient { get; set; }
        public LibraryClient LibraryClient { get; set; }
        public LibraryStructureClient LibraryStructureClient { get; set; }
        public LiveTvClient LiveTvClient { get; set; }
        public LocalizationClient LocalizationClient { get; set; }
        public MediaInfoClient MediaInfoClient { get; set; }
        public MoviesClient MoviesClient { get; set; }
        public MusicGenresClient MusicGenresClient { get; set; }
        public NotificationsClient NotificationsClient { get; set; }
        public PackageClient PackageClient { get; set; }
        public PersonsClient PersonsClient { get; set; }
        public PlaylistsClient PlaylistsClient { get; set; }
        public PlaystateClient PlaystateClient { get; set; }
        public PluginsClient PluginsClient { get; set; }
        public QuickConnectClient QuickConnectClient { get; set; }
        public RemoteImageClient RemoteImageClient { get; set; }
        public ScheduledTasksClient ScheduledTasksClient { get; set; }
        public SearchClient SearchClient { get; set; }
        public SessionClient SessionClient { get; set; }
        public StartupClient StartupClient { get; set; }
        public StudiosClient StudiosClient { get; set; }
        public SubtitleClient SubtitleClient { get; set; }
        public SuggestionsClient SuggestionsClient { get; set; }
        public SyncPlayClient SyncPlayClient { get; set; }
        public SystemClient SystemClient { get; set; }
        public TimeSyncClient TimeSyncClient { get; set; }
        public TrailersClient TrailersClient { get; set; }
        public TvShowsClient TvShowsClient { get; set; }
        public UniversalAudioClient UniversalAudioClient { get; set; }
        public UserClient UserClient { get; set; }
        public UserLibraryClient UserLibraryClient { get; set; }
        public UserViewsClient UserViewsClient { get; set; }
        public VideoAttachmentsClient VideoAttachmentsClient { get; set; }
        public VideoHlsClient VideoHlsClient { get; set; }
        public VideosClient VideosClient { get; set; }
        public YearsClient YearsClient { get; set; }

        #endregion
    }
}
