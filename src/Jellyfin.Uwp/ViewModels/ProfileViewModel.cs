using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Views;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        public UserDto UserDto { get; set; }
        public UserConfiguration UserConfiguration { get; set; }
        public UserPolicy UserPolicy { get; set; }
        public PublicSystemInfo PublicServerInfo { get; set; }
        public string BaseUrl { get; set; }
        public string AppVersion { get; set; }
        public string AppName { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }

        public DelegateCommand ChangeServerCommand { get; set; }

        public BitmapImage ProfileImageSource {get;set;}

        public ProfileViewModel()
        {
            UserDto = App.Current.AppUser.User;
            UserConfiguration = UserDto.Configuration;
            UserPolicy = UserDto.Policy;
            PublicServerInfo = App.Current.PublicSystemInfo;
            BaseUrl = App.Current.SdkClientSettings.BaseUrl;
            AppName = Constants.AppName;
            AppVersion = Constants.AppVersion;
            DeviceName = Constants.DeviceName;
            DeviceId = Constants.DeviceId.ToString();
            ChangeServerCommand = new DelegateCommand(async () => await AttemptServerChangeAsync());            
        }

        //public async Task PageReadyAsync()
        //{
            
        //}


        private async Task AttemptServerChangeAsync()
        {
            MessageDialog md = new MessageDialog("Are you sure you want to change the server?", "Change Server");

            md.Commands.Add(new UICommand("Change Server", (command) =>
            {
                // Clear the Stored Base Url in AppData
                StorageHelpers.Instance.SaveSetting("BaseUrl", "", Constants.JellyfinSettingsFile);
                StorageHelpers.Instance.SaveSetting("ProfileImageUri", "", Constants.JellyfinSettingsFile);

                // Clear the SdkSettings
                App.Current.SdkClientSettings.BaseUrl = null;
                App.Current.SdkClientSettings.AccessToken = null;

                // Navigate to Login will also clear Auth Token
                App.Current.RootFrame.Navigate(typeof(LoginPage));
            }));

            md.Commands.Add(new UICommand("Cancel"));

            md.DefaultCommandIndex = 2;

            await md.ShowAsync();
        }
    }
}
