using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.Models
{
    public class AppUser
    {
        public UserDto User { get; set; }
        public BitmapImage ProfileImage { get; set; }
    }
}
