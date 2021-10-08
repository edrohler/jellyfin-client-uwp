using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Models;
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
        public UserDto UserDto
        => App.Current.AppUser.User;

        public PublicSystemInfo PublicSystemInfo => App.Current.PublicSystemInfo;

        public string BaseUrl => App.Current.SdkClientSettings.BaseUrl;

        public string AppVersion => Constants.AppVersion;

        public string AppName => Constants.AppName;

        public string DeviceName => Constants.DeviceName;

        public string DeviceId => Constants.DeviceId.ToString();

        public DelegateCommand ChangeServerCommand { get; set; }

        public DelegateCommand ChangeProfilePicture { get; set; }

        public ProfileViewModel()
        {
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
                // Clear the Stored Profile Image in AppData
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
