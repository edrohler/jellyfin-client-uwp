using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
