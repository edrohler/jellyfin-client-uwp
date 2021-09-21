using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Jellyfin.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string serverUrl;
        private string username;
        private string password;
        private bool isValidServerUrl = true;
        private bool isServerUrlVisible = true;

        public LoginViewModel()
        {
            // The cool thing about using a "CanExecute" delegates,
            // is that it will automatically disable a bound UI control if it returns false
            LoginCommand = new DelegateCommand(async ()=> await LoginAsync(), CanLoginExecute);
            ForgotPasswordCommand = new DelegateCommand(async () => await ForgotPasswordAsync(), CanForgotPasswordExecute);
        }

        public string ServerUrl
        {
            get => serverUrl;
            set => SetProperty(ref serverUrl, value);
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

        public DelegateCommand LoginCommand { get; set; }

        public DelegateCommand ForgotPasswordCommand { get; set; }
        
        public async Task PageReadyAsync()
        {
            // Always Clear User's Credentials ?
            // StorageHelpers.Instance.DeleteToken(Constants.AccessTokenKey);

            //// If we also need to clear the server URL, take care of it in the two important locations
            //if (!IsServerUrlVisible)
            //{
            //    App.Current.DefaultHttpClient.BaseAddress = null;
            //    App.Current.SdkClientSettings.BaseUrl = "";
            //}

            if (string.IsNullOrEmpty(App.Current.SdkClientSettings.BaseUrl))
            {
                IsValidServerUrl = false;
                IsServerUrlVisible = true;
            } else
            {
                PublicSystemInfo ServerInfo = await ValidateServerAsync(App.Current.SdkClientSettings.BaseUrl);
                if (!string.IsNullOrEmpty(ServerInfo.Id))
                {
                    IsValidServerUrl = true;
                    IsValidServerUrl = false;
                }
            }
        }

        private async Task LoginAsync()
        {
            IsBusy = true;
            IsBusyMessage = "Logging in...";

            // We need to set the Server URL to the HttpClient's BaseAddress and the SdkSettings BaseUrl
            App.Current.DefaultHttpClient.BaseAddress = new Uri(this.ServerUrl, UriKind.Absolute);
            App.Current.SdkClientSettings.BaseUrl = this.ServerUrl;

            // Now, we can make a request to the server
            AuthenticationResult authenticationResult = await UserClientService.Current.UserLibraryClient.AuthenticateUserByNameAsync(new AuthenticateUserByName
            {
                Username = this.Username,
                Pw = this.Password
            });
            
            // Once we have the token, we can update the SdkClientSettings (do not save the SdkClient settings json when it has the token inside!!!)
            App.Current.SdkClientSettings.AccessToken = authenticationResult.AccessToken;

            
            // and then save the value securely
            StorageHelpers.Instance.StoreToken(Constants.AccessTokenKey, authenticationResult.AccessToken);

            // when you're done, go to the ShellPage (which has HomePage as the first page in the NavigationView)
            App.Current.Shell.Frame.Navigate(typeof(ShellPage), authenticationResult.User);

            IsBusyMessage = "";
            IsBusy = false;
        }
        
        // The Login method will not execute if IsValidServerUrl is false
        private bool CanLoginExecute()
        {
            return IsValidServerUrl;
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

            ForgotPasswordResult result =  await UserClientService.Current.UserLibraryClient.ForgotPasswordAsync(new ForgotPasswordDto
            {
                EnteredUsername = this.Username
            });

            IsBusyMessage = "";
            IsBusy = false;


            // I don't know how this part of the API works, this stuff is just an educated guess. You can easily update it to meet your needs
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

        public async Task<PublicSystemInfo> ValidateServerAsync(string baseurl)
        {
            return await SystemClientService.Current.SystemClient.GetPublicSystemInfoAsync();
        }
    }
}
