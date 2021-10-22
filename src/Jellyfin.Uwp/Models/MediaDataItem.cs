using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Jellyfin.Models
{
    public class MediaDataItem
    {
        public BaseItemDto BaseItem { get; set; }
        public TimeSpan UpdateInterval { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
