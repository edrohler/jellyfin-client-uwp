using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum MusicFilters
    {
        [DisplayName("Favorites")]
        IsFavorites,
        [DisplayName("Likes")]
        Likes,
        [DisplayName("Dislikes")]
        Dislikes
    }
}
