using System;
using System.Collections.Generic;

namespace Jellyfin.Models
{
    public class LatestMediaDataItems
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<LatestMediaDataItem> LatestItems { get; set; }
    }
}
