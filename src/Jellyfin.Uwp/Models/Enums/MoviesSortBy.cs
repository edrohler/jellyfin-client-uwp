using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum MoviesSortBy
    {
        [DisplayName("Name")]
        SortName,
        [DisplayName("IMDb Rating")]
        CommunityRating,
        [DisplayName("Critic Rating")]
        CriticRating,
        [DisplayName("Date Added")]
        DateCreated,
        [DisplayName("Date Played")]
        DatePlayed,
        [DisplayName("Parental Rating")]
        OfficialRating,
        [DisplayName("Play Count")]
        PlayCount,
        [DisplayName("Release Date")]
        PremiereDate,
        [DisplayName("Runtime")]
        Runtime,
        [DisplayName("Production Year")]
        ProductionYear
    }
}
