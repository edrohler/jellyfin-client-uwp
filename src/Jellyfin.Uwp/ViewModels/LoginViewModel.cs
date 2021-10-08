using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Models;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Jellyfin.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string serverUrl;
        private string username;
        private string password;
        private string serverUrlHeader;
        private bool isValidServerUrl = true;
        private bool isServerUrlVisible;
        private bool showServerConnectionChangeButton;
        private string serverConnectionString;
        private string authErrorString;

        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand(async () => await LoginAsync());
            ForgotPasswordCommand = new DelegateCommand(async () => await ForgotPasswordAsync(), CanForgotPasswordExecute);
            ClearServerConnectionCommand = new DelegateCommand(ClearServerConnection);
        }

        public string ServerConnectionString
        {
            get => serverConnectionString;
            set => SetProperty(ref serverConnectionString, value);
        }

        public string AuthErrorString
        {
            get => authErrorString;
            set => SetProperty(ref authErrorString, value);
        }

        public string ServerUrl
        {
            get => serverUrl;
            set => SetProperty(ref serverUrl, value, nameof(ServerUrl), ValidateServer);
        }

        public string ServerUrlHeader
        {
            get => serverUrlHeader;
            set => SetProperty(ref serverUrlHeader, value);
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public bool IsValidServerUrl
        {
            get => isValidServerUrl;
            set => SetProperty(ref isValidServerUrl, value);
        }

        public bool IsServerUrlVisible
        { 
            get => isServerUrlVisible;
            set => SetProperty(ref isServerUrlVisible, value);
        }

        public PublicSystemInfo ServerSystemInfo
        {
            get => App.Current.PublicSystemInfo;
            set => App.Current.PublicSystemInfo = value;
        }

        public bool ShowServerConnectionChangeButton
        {
            get => showServerConnectionChangeButton;
            set => SetProperty(ref showServerConnectionChangeButton, value);
        }

        public DelegateCommand LoginCommand { get; set; }

        public DelegateCommand ForgotPasswordCommand { get; set; }

        public DelegateCommand ClearServerConnectionCommand { get; set; }

        public async Task PageReadyAsync()
        {
            // Always Clear User's Credentials
            StorageHelpers.Instance.DeleteToken(Constants.AccessTokenKey);

            // Sets BaseUrl on Login or from settings on App Startup.
            // If it's not present, show the ServerUrl Input.
            if (string.IsNullOrEmpty(App.Current.SdkClientSettings.BaseUrl))
            {
                IsValidServerUrl = false;
                IsServerUrlVisible = true;
                ShowServerConnectionChangeButton = false;
            }
            else
            {
                ServerSystemInfo = await JellyfinClientServices.Current.SystemClient.GetPublicSystemInfoAsync();
                if (!string.IsNullOrEmpty(ServerSystemInfo.Id))
                {
                    IsValidServerUrl = true;
                    IsServerUrlVisible = false;
                    ShowServerConnectionChangeButton = true;
                    ServerUrl = App.Current.SdkClientSettings.BaseUrl;
                    ServerConnectionString = $"Change {ServerUrl} Connection?";
                }
            }
        }

        private async Task LoginAsync()
        {
            IsBusy = true;
            IsBusyMessage = "Logging in...";
            AuthErrorString = "";
            try
            {
                // Make a login request to the server
                AuthenticationResult authenticationResult = await JellyfinClientServices.Current.UserClient.AuthenticateUserByNameAsync(
                    new AuthenticateUserByName
                    {
                        Username = Username,
                        Pw = Password
                    });

                // Update the SdkClientSettings with AccessToken
                App.Current.SdkClientSettings.AccessToken = authenticationResult.AccessToken;

                // Encrypot and save the token
                StorageHelpers.Instance.StoreToken(Constants.AccessTokenKey, authenticationResult.AccessToken);

                // Set the App Current User
                App.Current.AppUser = new AppUser
                {
                    User = await JellyfinClientServices.Current.UserClient.GetCurrentUserAsync()
                };

                // Navigate to the ShellPage passing in the UserDto
                App.Current.RootFrame.Navigate(typeof(ShellPage));

                IsBusyMessage = "";
                IsBusy = false;
            }
            catch (Exception ex)
            {
                AuthErrorString = ex.Message;
                // Log Auth Error
                ExceptionLogger.LogException(ex);
            }
        }

        private async Task ForgotPasswordAsync()
        {
            if (string.IsNullOrEmpty(ServerUrl))
            {
                await new MessageDialog("You must complete the Server URL in order to reset your password").ShowAsync();
                return;
            }
            else
            {
                // Set the server Url so the user can reset password
                App.Current.DefaultHttpClient.BaseAddress = new Uri(ServerUrl, UriKind.Absolute);
                App.Current.SdkClientSettings.BaseUrl = ServerUrl;
            }

            IsBusy = true;
            IsBusyMessage = "Resetting password...";

            ForgotPasswordResult result = await JellyfinClientServices.Current.UserClient.ForgotPasswordAsync(new ForgotPasswordDto
            {
                EnteredUsername = Username
            });

            IsBusyMessage = "";
            IsBusy = false;

            // I don't know how this part of the API works, this stuff is just an educated guess.
            // You can easily update it to meet your needs
            switch (result.Action)
            {
                case ForgotPasswordAction.PinCode:
                    result.PinFile = Constants.PinFilePath;
                    await new MessageDialog($"Check your email for a PIN, then use that PIN for your password (expires {result.PinExpirationDate:g})").ShowAsync();
                    break;
                case ForgotPasswordAction.ContactAdmin:
                    await new MessageDialog("Contact your server admin to reset your password.").ShowAsync();
                    break;
                case ForgotPasswordAction.InNetworkRequired:
                    await new MessageDialog("You can only reset your password when on the same network as the server.").ShowAsync();
                    break;
            }
        }

        private bool CanForgotPasswordExecute()
        {
            // the ForgotPassword method will not execute if the Username is empty
            return !string.IsNullOrEmpty(Username);
        }

        private void ClearServerConnection()
        {
            // Clear the Stored Base Url in AppData
            StorageHelpers.Instance.SaveSetting("BaseUrl", "", Constants.JellyfinSettingsFile);
            // Clear the Stored Profile Image in AppData
            StorageHelpers.Instance.SaveSetting("ProfileImageUri", "", Constants.JellyfinSettingsFile);

            AuthErrorString = "";
            ServerSystemInfo = null;
            App.Current.SdkClientSettings.BaseUrl = null;
            App.Current.SdkClientSettings.AccessToken = null;
            ServerUrl = "";

            IsValidServerUrl = false;
            IsServerUrlVisible = true;
            ShowServerConnectionChangeButton = false;
        }

        private async void ValidateServer()
        {
            if (!string.IsNullOrEmpty(ServerUrl))
            {
                // Check if valid URI for HttpClient
                bool result = Uri.TryCreate(ServerUrl, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (result)
                {
                    try
                    {
                        // Set default http client base address.
                        // Will throw if a previous singletone instance was configured
                        App.Current.DefaultHttpClient.BaseAddress = uriResult;
                    }
                    catch (Exception ex)
                    {
                        // Have to re-configure the DefaultHttpClient when Server URL is changed
                        // Otherwise, it will throw because we're using a singleton
                        ExceptionLogger.LogException(ex);
                        App.Current.ConfigureDefaultHttpClient();
                    }
                    finally
                    {
                        // Update App.Current Settings in Memory
                        App.Current.SdkClientSettings.BaseUrl = ServerUrl;

                        try
                        {
                            // Update ValidServerUrl on Successful GetPublicSyInfo call
                            // Will throw if a Jellyfin server is not available
                            PublicSystemInfo ServerInfo = await JellyfinClientServices.Current.SystemClient.GetPublicSystemInfoAsync();

                            // Update settings to local storage on Successful GetPublicSysInfo call
                            StorageHelpers.Instance.SaveSetting("BaseUrl", ServerUrl, Constants.JellyfinSettingsFile);

                            // Enable Login button
                            IsValidServerUrl = true;
                            ServerUrlHeader = "Valid Jellyfin Server";
                        }
                        catch (Exception ex)
                        {
                            // Catch BadRequest to GetPublicSysInfo
                            // This means likely good URI but not a Jellyfin Server
                            ServerUrlHeader = "Not a valid Jellyfin Server";
                            ExceptionLogger.LogException(ex);
                            App.Current.SdkClientSettings.BaseUrl = "";
                            IsValidServerUrl = false;
                        }
                    }
                }
                else
                {
                    ServerUrlHeader = "Not a valid URI";

                    ExceptionLogger.LogException(new Exception(ServerUrlHeader)
                    {
                        Source = $"{AppDomain.CurrentDomain.FriendlyName} - {GetType().Name} - {MethodBase.GetCurrentMethod()}"
                    });

                    App.Current.SdkClientSettings.BaseUrl = "";
                    IsValidServerUrl = false;
                }
            }
            else
            {
                ServerUrlHeader = "URI is Empty";

                ExceptionLogger.LogException(new Exception(ServerUrlHeader)
                {
                    Source = $"{AppDomain.CurrentDomain.FriendlyName} - {GetType().Name} - {MethodBase.GetCurrentMethod()}"
                });
                App.Current.SdkClientSettings.BaseUrl = "";
                IsValidServerUrl = false;
            }
        }
    }
}
