using Jellyfin.Helpers;
using Jellyfin.ViewModels;
using System;
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

        private void OnKeyUpHandler(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // Capture Enter Key Up
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Get DataContext
                LoginViewModel vm = (LoginViewModel)((Grid)sender).DataContext;
                // Login
                vm.LoginCommand.Execute(null);
            }
        }
    }
}
