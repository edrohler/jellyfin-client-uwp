using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            this.InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Clear User's Credentials
            StorageHelpers.Instance.DeleteToken(Constants.DeviceName);
            ContentFrame.Navigate(typeof(LoginPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            // Get Access Token 
            // If that exists then nav to MainPage
            if (string.IsNullOrEmpty(StorageHelpers.Instance.LoadToken(Constants.DeviceName)))
            {
                // if Not then login
                ContentFrame.Navigate(typeof(LoginPage));
            } else
            {
                ContentFrame.Navigate(typeof(MainPage));
            }

        }
    }
}
