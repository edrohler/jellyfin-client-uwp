using CommonHelpers.Common;
using Jellyfin.Sdk;

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
