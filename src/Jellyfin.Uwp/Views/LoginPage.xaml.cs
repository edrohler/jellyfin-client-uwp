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
            
            await ViewModel.PageReadyAsync();
        }
    }
}
