using System.Collections.Generic;

namespace Jellyfin.Models
{
    public class LatestMedia : MediaDataItem
    {
        public string Name { get; set; }
        public List<MediaDataItem> LatestItems { get; set; }
    }
}
