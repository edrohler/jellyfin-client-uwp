using CommonHelpers.Common;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.ViewModels
{
    public class SuggestionsViewModel : ViewModelBase
    {
        private BaseItemDto userView;
        private BaseItemDtoQueryResult query;

        public BaseItemDto UserView { get => userView; set => SetProperty(ref userView, value); }
        public BaseItemDtoQueryResult Query { get => query; set => SetProperty(ref query, value); }

        public ObservableCollection<MediaDataItemCollection> SuggestionsCollection { get; set; }

        public SuggestionsViewModel()
        {
            SuggestionsCollection = new ObservableCollection<MediaDataItemCollection>();
        }

        public async Task PageReadyAsync()
        {
            IsBusy = true;

            switch (UserView.CollectionType)
            {
                case "movies":
                    /*
                     * Continue Watching Collection - ItemsClient
                     * Latest Movies Collection - UserLibraryClient
                     * Recommendations Collection - MoviesCLient
                     */

                    // Continue Watching
                    BaseItemDtoQueryResult MoviesContinueWatching = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        parentId: UserView.Id,
                        sortBy: new string[]
                        {
                            "DatePlayed"
                        },
                        sortOrder: new SortOrder[]
                        {
                            SortOrder.Descending
                        },
                        filters: new ItemFilter[]
                        {
                            ItemFilter.IsResumable
                        },
                        recursive: true,
                        collapseBoxSetItems: false,
                        includeItemTypes: new string[]
                        { 
                            "Movie"
                        },
                        limit: 10,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.MediaSourceCount,
                            ItemFields.BasicSyncInfo
                        },
                        imageTypeLimit:1,
                        enableImageTypes: new ImageType[]
                        { 
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Banner,
                            ImageType.Thumb
                        },
                        enableTotalRecordCount:false);

                    if (MoviesContinueWatching.TotalRecordCount > 0)
                    {
                        List<MediaDataItem> movieList = new List<MediaDataItem>();

                        foreach (BaseItemDto item in MoviesContinueWatching.Items)
                        {
                            movieList.Add(new MediaDataItem
                            {
                                BaseItem = item,
                                Height = 450,
                                Width = 300
                            });
                        }

                        SuggestionsCollection.Add(new MediaDataItemCollection
                        {
                            Name = "Continue Watching",
                            LatestItems = movieList
                        });
                    }

                    // Latest Movies
                    IReadOnlyList<BaseItemDto> MoviesLatestItems = await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(
                        App.Current.AppUser.User.Id,
                        includeItemTypes: new string[]
                        {
                            "Movie"
                        },
                        limit: 18,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.MediaSourceCount,
                            ItemFields.BasicSyncInfo
                        },
                        parentId: UserView.Id,
                        imageTypeLimit: 1,
                        enableImageTypes: new ImageType[]
                        {
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Banner,
                            ImageType.Thumb
                        });

                    if (MoviesLatestItems.Count > 0)
                    {
                        List<MediaDataItem> latestMovies = new List<MediaDataItem>();
                        foreach (BaseItemDto item in MoviesLatestItems)
                        {
                            latestMovies.Add(new MediaDataItem
                            {
                                BaseItem = item,
                                Height = 450,
                                Width = 300
                            });
                        }

                        SuggestionsCollection.Add(new MediaDataItemCollection
                        {
                            Name = "Latest Movies",
                            LatestItems = latestMovies
                        });
                    }

                    // Recommendations
                    MoviesClient MoviesClient = new MoviesClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
                    IReadOnlyList<RecommendationDto> MovieRecommendations = await MoviesClient.GetMovieRecommendationsAsync(
                        userId: App.Current.AppUser.User.Id,
                        parentId: UserView.Id,
                        categoryLimit: 6,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.MediaSourceCount,
                            ItemFields.BasicSyncInfo
                        });

                    if (MovieRecommendations.Count > 0)
                    {
                        foreach (RecommendationDto recoItem in MovieRecommendations)
                        {
                            List<MediaDataItem> recoList;

                            switch (recoItem.RecommendationType)
                            {
                                case RecommendationType.SimilarToRecentlyPlayed:
                                    recoList = new List<MediaDataItem>();

                                    foreach (BaseItemDto item in recoItem.Items)
                                    {
                                        recoList.Add(new MediaDataItem
                                        {
                                            BaseItem = item,
                                            Height = 450,
                                            Width = 300
                                        });
                                    }

                                    SuggestionsCollection.Add(new MediaDataItemCollection
                                    {
                                        Name = $"Because you watched {recoItem.BaselineItemName}",
                                        LatestItems = recoList
                                    });

                                    break;
                                case RecommendationType.SimilarToLikedItem:
                                    recoList = new List<MediaDataItem>();

                                    foreach (BaseItemDto item in recoItem.Items)
                                    {
                                        recoList.Add(new MediaDataItem
                                        {
                                            BaseItem = item,
                                            Height = 450,
                                            Width = 300
                                        });
                                    }

                                    SuggestionsCollection.Add(new MediaDataItemCollection
                                    {
                                        Name = $"Because you liked {recoItem.BaselineItemName}",
                                        LatestItems = recoList
                                    });

                                    break;
                                case RecommendationType.HasDirectorFromRecentlyPlayed:
                                    recoList = new List<MediaDataItem>();

                                    foreach (BaseItemDto item in recoItem.Items)
                                    {
                                        recoList.Add(new MediaDataItem
                                        {
                                            BaseItem = item,
                                            Height = 450,
                                            Width = 300
                                        });
                                    }

                                    SuggestionsCollection.Add(new MediaDataItemCollection
                                    {
                                        Name = $"Directed by {recoItem.BaselineItemName}",
                                        LatestItems = recoList
                                    });

                                    break;
                                case RecommendationType.HasActorFromRecentlyPlayed:
                                    recoList = new List<MediaDataItem>();

                                    foreach (BaseItemDto item in recoItem.Items)
                                    {
                                        recoList.Add(new MediaDataItem
                                        {
                                            BaseItem = item,
                                            Height = 450,
                                            Width = 300
                                        });
                                    }

                                    SuggestionsCollection.Add(new MediaDataItemCollection
                                    {
                                        Name = $"Starring {recoItem.BaselineItemName}",
                                        LatestItems = recoList
                                    });

                                    break;
                                default:
                                    recoList = new List<MediaDataItem>();

                                    foreach (BaseItemDto item in recoItem.Items)
                                    {
                                        recoList.Add(new MediaDataItem
                                        {
                                            BaseItem = item,
                                            Height = 450,
                                            Width = 300
                                        });
                                    }

                                    SuggestionsCollection.Add(new MediaDataItemCollection
                                    {
                                        Name = $"Because you liked {recoItem.BaselineItemName}",
                                        LatestItems = recoList
                                    });

                                    break;
                            }
                        }
                    }

                    break;
                case "tvshows":
                    /* 
                     * Continue Watching Collection - ItemsClient
                     * Latest Episodes Collection - UserLibraryClient
                     * Next Up Collectionn - TvShowsClient
                    */

                    // Continue Watching
                    BaseItemDtoQueryResult TvShowsContinueWatching = await JellyfinClientServices.Current.ItemsClient.GetItemsByUserIdAsync(
                        App.Current.AppUser.User.Id,
                        parentId: UserView.Id,
                        sortBy: new string[]
                        {
                            "DatePlayed"
                        },
                        sortOrder: new SortOrder[]
                        {
                            SortOrder.Descending
                        },
                        includeItemTypes: new string[]
                        {
                            "Episode"
                        },
                        recursive: true,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.MediaSourceCount,
                            ItemFields.BasicSyncInfo
                        },
                        collapseBoxSetItems: false,
                        filters: new ItemFilter[]
                        {
                            ItemFilter.IsResumable
                        },
                        limit: 1,
                        enableImageTypes: new ImageType[]
                        { 
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Banner,
                            ImageType.Thumb
                        },
                        enableTotalRecordCount: false);

                    if (TvShowsContinueWatching.Items.Count > 0)
                    {
                        List<MediaDataItem> tvShowsList = new List<MediaDataItem>();

                        foreach (BaseItemDto item in TvShowsContinueWatching.Items)
                        {
                            tvShowsList.Add(new MediaDataItem
                            {
                                BaseItem = item,
                                Height = 253,
                                Width = 450
                            });
                        }

                        SuggestionsCollection.Add(new MediaDataItemCollection
                        {
                            Name = "Continue Watching",
                            LatestItems = tvShowsList
                        });
                    }

                    // Latest Episodes
                    IReadOnlyList<BaseItemDto> TvShowsLatestItems = await JellyfinClientServices.Current.UserLibraryClient.GetLatestMediaAsync(
                        App.Current.AppUser.User.Id,
                        parentId: UserView.Id,
                        includeItemTypes: new string[]
                        {
                            "Episode"
                        },
                        limit: 30,
                        fields: new ItemFields[]
                        {
                            ItemFields.PrimaryImageAspectRatio,
                            ItemFields.BasicSyncInfo
                        },
                        imageTypeLimit: 1,
                        enableImageTypes: new ImageType[]
                        { 
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Thumb
                        });

                    if (TvShowsLatestItems.Count > 0)
                    {
                        List<MediaDataItem> latestTvShows = new List<MediaDataItem>();

                        foreach (BaseItemDto item in TvShowsLatestItems)
                        {
                            latestTvShows.Add(new MediaDataItem
                            {
                                BaseItem = item,
                                Height = 450,
                                Width = 300
                            });
                        }

                        SuggestionsCollection.Add(new MediaDataItemCollection
                        {
                            Name = "Latest TV Shows",
                            LatestItems = latestTvShows
                        });
                    }

                    // Next Up Episodes
                    TvShowsClient TvShowsClient = new TvShowsClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);
                    BaseItemDtoQueryResult NextUpEpisodes = await TvShowsClient.GetNextUpAsync(
                        userId: App.Current.AppUser.User.Id,
                        parentId: UserView.Id,
                        limit: 24,
                        imageTypeLimit: 1,
                        enableImageTypes: new ImageType[]
                        {
                            ImageType.Primary,
                            ImageType.Backdrop,
                            ImageType.Thumb
                        },
                        enableTotalRecordCount: false);

                    if(NextUpEpisodes.Items.Count > 0)
                    {
                        List<MediaDataItem> nextUpList = new List<MediaDataItem>();

                        foreach (BaseItemDto item in NextUpEpisodes.Items)
                        {
                            nextUpList.Add(new MediaDataItem
                            {
                                BaseItem = item,
                                Height = 253,
                                Width = 450
                            });
                        }

                        SuggestionsCollection.Add(new MediaDataItemCollection
                        {
                            Name = "Next Up",
                            LatestItems = nextUpList
                        });
                    }

                    break;
                case "music":
                    /*
                     * Latest Music Collection - UserLibraryClient
                     * Recently Played Collection - ItemsClient
                     * Frequently Played Collection - ItemsClient
                     * Favorite Artists - ArtistsClient
                     * Favorite Songs - ItemsClient
                     * Favorite Albums - ItemsClient
                     */


                    break;
                default:
                    break;
            }

            IsBusy = false;
        }
    }
}
