namespace Jellyfin.Helpers
{
    public static class IconHelper
    {
        // Constants for commonly used Segoe Icons //
        // To create more, see docs for full list https://docs.microsoft.com/en-us/windows/apps/design/style/segoe-ui-symbol-font

        public static string HomeIcon = "\uE80F";
        public static string BooksIcon = "\uE736";
        public static string MoviesIcon = "\uE8B2";
        public static string TvIcon = "\uE7F4";
        public static string MusicIcon = "\uE8D6";
        public static string PhotosIcon = "\uEB9F";
        public static string CollectionsIcon = "\uF168";
        
        public static string GetIconForItem(string name)
        {
            switch (name)
            {
                case "books":
                    return BooksIcon;
                case "tvshows":
                    return TvIcon;
                case "movies":
                    return MoviesIcon;
                case "music":
                    return MusicIcon;
                case "homevideos":
                    return PhotosIcon;
                case "boxsets":
                    return CollectionsIcon;
                default:
                    return HomeIcon;
            }
        }
    }
}
