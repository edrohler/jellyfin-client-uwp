using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum TvShowsFilters
    {
        [DisplayName("Continue Watching")]
        IsResumable,
        [DisplayName("Favorites")]
        IsFavorite,
        [DisplayName("Played")]
        IsPlayed,
        [DisplayName("Unplayed")]
        IsUnPlayed,
        [DisplayName("Likes")]
        Likes,
        [DisplayName("Dislikes")]
        Dislikes
    }
}
