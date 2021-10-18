using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum FolderFilters
    {
        [DisplayName("Continue Watching")]
        IsResumable,
        [DisplayName("Favorites")]
        IsFavorite,
        [DisplayName("Played")]
        IsPlayed,
        [DisplayName("Unplayed")]
        IsUnPlayed
    }
}
