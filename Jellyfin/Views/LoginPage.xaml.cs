using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Clear User's Credentials
            StorageHelpers.Instance.DeleteToken(Constants.AccessTokenKey);

            // Determine if we need to show the Server URL box
            if (e.Parameter is LogoutType logoutType)
            {
                switch (logoutType)
                {
                    case LogoutType.User:
                        ViewModel.IsServerUrlVisible = false;
                        break;
                    case LogoutType.Server:
                        ViewModel.IsServerUrlVisible = true;
                        break;
                }
            }


            await ViewModel.PageReadyAsync();
        }
    }
}
