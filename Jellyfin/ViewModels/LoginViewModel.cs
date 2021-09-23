using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Jellyfin.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string serverUrl;
        private string username;
        private string password;
        private string serverUrlHeader;
        private bool isValidServerUrl = true;
        public bool IsServerUrlVisible { get; set; }

        public PublicSystemInfo ServerSystemInfo;

        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand(async () => await LoginAsync());
            ForgotPasswordCommand = new DelegateCommand(async () => await ForgotPasswordAsync(), CanForgotPasswordExecute);
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

        public DelegateCommand LoginCommand { get; set; }

        public DelegateCommand ForgotPasswordCommand { get; set; }

        public async Task PageReadyAsync()
        {
            // Always Clear User's Credentials
            StorageHelpers.Instance.DeleteToken(Constants.AccessTokenKey);

            // Sets BaseUrl on Login or from settings on App Startup.
            // If it's not present, show the ServerUrl Input.
            if (string.IsNullOrEmpty(App.Current.SdkClientSettings.BaseUrl))
            {
                this.IsValidServerUrl = false;
                this.IsServerUrlVisible = true;
            }
            else
            {

                ServerSystemInfo = await SystemClientService.Current.SystemClient.GetPublicSystemInfoAsync();
                if (!string.IsNullOrEmpty(ServerSystemInfo.Id))
                {
                    this.IsValidServerUrl = true;
                    this.IsServerUrlVisible = false;
                }
            }
        }

        private async Task LoginAsync()
        {
            IsBusy = true;
            IsBusyMessage = "Logging in...";

            // Make a login request to the server
            AuthenticationResult authenticationResult = await UserClientService.Current.UserLibraryClient.AuthenticateUserByNameAsync(
                new AuthenticateUserByName
                {
                    Username = this.Username,
                    Pw = this.Password
                });

            // Update the SdkClientSettings with AccessToken
            App.Current.SdkClientSettings.AccessToken = authenticationResult.AccessToken;

            // Encrypot and save the token
            StorageHelpers.Instance.StoreToken(Constants.AccessTokenKey, authenticationResult.AccessToken);

            // Navigate to the ShellPage passing in the UserDto
            App.Current.RootFrame.Navigate(typeof(ShellPage), authenticationResult.User);

            IsBusyMessage = "";
            IsBusy = false;
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
                App.Current.DefaultHttpClient.BaseAddress = new Uri(this.ServerUrl, UriKind.Absolute);
                App.Current.SdkClientSettings.BaseUrl = this.ServerUrl;
            }

            IsBusy = true;
            IsBusyMessage = "Resetting password...";

            ForgotPasswordResult result = await UserClientService.Current.UserLibraryClient.ForgotPasswordAsync(new ForgotPasswordDto
            {
                EnteredUsername = this.Username
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


        public async void ValidateServer()
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
                        // Parse settings from local storage
                        JObject json = JObject.Parse(File.ReadAllText(Constants.JellyfinSettingsFile));
                        json["BaseUrl"] = ServerUrl;

                        // Update App.Current Settings in Memory
                        App.Current.SdkClientSettings.BaseUrl = ServerUrl;

                        try
                        {
                            // Update ValidServerUrl on Successful GetPublicSyInfo call
                            // Will throw if a Jellyfin server is not available
                            PublicSystemInfo ServerInfo = await SystemClientService.Current.SystemClient.GetPublicSystemInfoAsync();

                            // Update settings to local storage on Successful GetPublicSysInfo call
                            File.WriteAllText(Constants.JellyfinSettingsFile, json.ToString());

                            // Enable Login button
                            this.IsValidServerUrl = true;
                            ServerUrlHeader = "Valid Jellyfin Server";
                        }
                        catch (Exception ex)
                        {
                            // Catch BadRequest to GetPublicSysInfo
                            // This means likely good URI but not a Jellyfin Server
                            ServerUrlHeader = "Not a valid Jellyfin Server";
                            ExceptionLogger.LogException(ex);
                            App.Current.SdkClientSettings.BaseUrl = "";
                            this.IsValidServerUrl = false;
                        }
                    }
                }
                else
                {
                    ServerUrlHeader = "Not a valid URI";
                    ExceptionLogger.LogException(new Exception("Server URL Not Valid"));
                    App.Current.SdkClientSettings.BaseUrl = "";
                    this.IsValidServerUrl = false;
                }
            }
            else
            {
                ServerUrlHeader = "URI is Empty";
                ExceptionLogger.LogException(new Exception("Server URL is Null"));
                App.Current.SdkClientSettings.BaseUrl = "";
                this.IsValidServerUrl = false;
            }
        }
    }
}
