namespace Jellyfin.Helpers
{
    public static class IconHelper
    {
        // Constants for commonly used Segoe Icons //
        // To create more, see docs for full list https://docs.microsoft.com/en-us/windows/apps/design/style/segoe-ui-symbol-font

        public static string HomeIcon = "\uE80F";
        public static string MoviesIcon = "\uE8B2";
        public static string TvIcon = "\uE7F4";
        public static string MusicIcon = "\uE8D6";
        public static string PhotosIcon = "\uEB9F";
        public static string GamesIcon = "\uE7FC";
        
        public static string GetIconForItem(string name)
        {
            switch (name)
            {
                case "Home":
                    return HomeIcon;
                case "TV":
                    return TvIcon;
                case "Movies":
                    return MoviesIcon;
                case "Music":
                    return MusicIcon;
                case "Photos":
                    return PhotosIcon;
                case "Games":
                    return GamesIcon;
                default:
                    return "";
            }
        }
    }
}
