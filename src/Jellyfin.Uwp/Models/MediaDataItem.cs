using Jellyfin.Sdk;
using System;

namespace Jellyfin.Models
{
    public class MediaDataItem
    {
        public BaseItemDto BaseItem { get; set; }
        public TimeSpan UpdateInterval { get; set; }
        public Guid ImageId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int UnwatchedCount { get; set; }
    }
}
