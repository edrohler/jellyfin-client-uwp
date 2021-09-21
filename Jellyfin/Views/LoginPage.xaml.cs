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
            
            //// Determine if we need to show the Server URL box
            //if (e.Parameter is LogoutType logoutType)
            //{
            //    switch (logoutType)
            //    {
            //        // If we only logout the user, hide the Server URL box
            //        case LogoutType.User:
            //            ViewModel.IsServerUrlVisible = false;
            //            break;
            //        // If we're also changing servers, show the Server URL box
            //        case LogoutType.Server:
            //            ViewModel.IsServerUrlVisible = true;
            //            break;
            //    }
            //}

            await ViewModel.PageReadyAsync();
        }
    }
}
