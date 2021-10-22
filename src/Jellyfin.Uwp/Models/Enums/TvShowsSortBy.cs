using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum TvShowsSortBy
    {
        [DisplayName("Name")]
        SortName,
        [DisplayName("IMDb Rating")]
        CommunityRating,
        [DisplayName("Date Added")]
        DateCreated,
        [DisplayName("Date Played")]
        DatePlayed,
        [DisplayName("Parental Rating")]
        OfficialRating,
        [DisplayName("Release Date")]
        PremiereDate
    }
}
