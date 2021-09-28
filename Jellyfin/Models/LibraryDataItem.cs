using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.Models
{
    public class LibraryDataItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public BitmapImage ImageSrc { get; set; }
    }
}
