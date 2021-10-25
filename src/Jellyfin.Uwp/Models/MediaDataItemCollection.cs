using System.Collections.Generic;

namespace Jellyfin.Models
{
    public class MediaDataItemCollection : MediaDataItem
    {
        public string Name { get; set; }
        public List<MediaDataItem> LatestItems { get; set; }
    }
}
