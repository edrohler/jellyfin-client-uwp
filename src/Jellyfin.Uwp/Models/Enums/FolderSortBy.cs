using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum FolderSortBy
    {
        [DisplayName("Name")]
        SortName,
        [DisplayName("Community Rating")]
        CommunityRating,
        [DisplayName("Critic Rating")]
        CriticRating,
        [DisplayName("Date Added")]
        DateCreated,
        [DisplayName("Date Played")]
        DatePlayed,
        [DisplayName("Folders")]
        IsFolder,
        [DisplayName("Parental Rating")]
        OfficialRating,
        [DisplayName("Play Count")]
        PlayCount,
        [DisplayName("Release Date")]
        PremiereDate,
        [DisplayName("Runtime")]
        Runtime
    }
}
