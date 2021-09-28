using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Jellyfin.Models
{
    public class LatestMediaDataItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ImageSource ImageSrc { get; set; }
    }
}
