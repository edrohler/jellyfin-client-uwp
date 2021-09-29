using System;
using Windows.UI.Xaml.Media;

namespace Jellyfin.Models
{
    public class LatestMediaDataItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ImageSource ImageSrc { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }
}
