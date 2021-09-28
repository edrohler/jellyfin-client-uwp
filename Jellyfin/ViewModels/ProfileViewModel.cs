using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        public UserDto UserDto { get; set; }
        public UserConfiguration UserConfiguration { get; set; }
        public UserPolicy UserPolicy { get; set; }
        public ProfileViewModel()
        {
            UserDto = App.Current.AppUser;
            UserConfiguration = UserDto.Configuration;
            UserPolicy = UserDto.Policy;
        }

        //public async Task PageReadyAsync()
        //{
        //    var asdf = await UserClientService.Current.UserClient.GetCurrentUserAsync();

        //    var fdsa = UserConfiguration

        //    Console.WriteLine();
        //}
    }
}
