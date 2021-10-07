using CommonHelpers.Common;
using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.Models
{
    public class AppUser : BindableBase
    {
        private BitmapImage profileImage;
        private UserDto user;

        public UserDto User { get => user; set => SetProperty(ref user, value); }
        public BitmapImage ProfileImage { get => profileImage; set => SetProperty(ref profileImage, value); }
    }
}
