using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

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

        public DelegateCommand ChangeProfilePictureCommand { get; set; }

        public ProfileViewModel()
        {
            ChangeServerCommand = new DelegateCommand(async () => await AttemptServerChangeAsync());

            ChangeProfilePictureCommand = new DelegateCommand(async () => await AttemptChangeProfilePicture());
        }

        private async Task AttemptChangeProfilePicture()
        {
            FileOpenPicker FileDialog = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            FileDialog.FileTypeFilter.Add(".jpg");
            FileDialog.FileTypeFilter.Add(".jpeg");
            FileDialog.FileTypeFilter.Add(".png");

            StorageFile Image = await FileDialog.PickSingleFileAsync();
            if (Image != null)
            {
                try
                {
                    // Try Uploading to Server                    
                    using (Stream stream = await Image.OpenStreamForReadAsync())
                    {
                        // CopyTo a MemoryStream for Base64 Conversion
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // Copy stream to memory stream
                            await stream.CopyToAsync(ms);
                            ms.Position = 0;

                            // Convert File to base64
                            byte[] bytes = ms.ToArray();
                            string base64 = Convert.ToBase64String(bytes);
                            // Convert to Stream
                            Stream WebFileStream = new MemoryStream(Encoding.UTF8.GetBytes(base64));
                            // Create FileParameter
                            FileParameter fp = new FileParameter(WebFileStream, Image.Name, Image.ContentType);

                            // Post Image to Server
                            await JellyfinClientServices.Current.ImageClient.PostUserImageAsync(
                                App.Current.AppUser.User.Id, ImageType.Profile, 0, fp);

                            // Update AppUser BitmapImage Binding
                            await App.Current.AppUser.ProfileImage.SetSourceAsync(ms.AsRandomAccessStream());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                }
            }
        }

        private async Task AttemptServerChangeAsync()
        {
            MessageDialog md = new MessageDialog("Are you sure you want to change the server?", "Change Server");

            md.Commands.Add(new UICommand("Change Server", (command) =>
            {
                // Clear the Stored Base Url in AppData
                StorageHelpers.Instance.SaveSetting("BaseUrl", "", Constants.JellyfinSettingsFile);
                //// Clear the Stored Profile Image in AppData
                //StorageHelpers.Instance.SaveSetting("ProfileImageUri", "", Constants.JellyfinSettingsFile);

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
