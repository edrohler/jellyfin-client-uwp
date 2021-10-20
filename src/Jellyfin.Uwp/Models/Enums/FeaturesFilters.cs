using System.ComponentModel;

namespace Jellyfin.Models.Enums
{
    public enum FeaturesFilters
    {
        [DisplayName("Subtitle")]
        Subtitles,
        [DisplayName("Trailer")]
        Trailer,
        [DisplayName("Special Features")]
        SpecialFeatures,
        [DisplayName("Theme Song")]
        ThemeSong,
        [DisplayName("Theme Video")]
        ThemeVideo
    }
}
