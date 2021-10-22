using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum MusicSortBy
    {
        [DisplayName("Name")]
        SortName,
        [DisplayName("Album Artist")]
        AlbumArtist,
        [DisplayName("Community Rating")]
        CommunityRating,
        [DisplayName("Critic Rating")]
        CriticRating,
        [DisplayName("Date Added")]
        DateCreated,
        [DisplayName("Release Date")]
        PremiereDate,
        [DisplayName("Random")]
        Random
    }
}
