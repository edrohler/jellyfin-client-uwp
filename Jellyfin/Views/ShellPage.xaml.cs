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

        private string JellyfinSettingsFile = $"{ApplicationData.Current.LocalFolder.Path}\\Jellyfin-UWP-{Environment.MachineName}.json";
        private string ClientName = "Jellyfin Universal Windows";
        // TODO: Read this from a build config.
        private string ClientVersion = "0.0.1";
        private string DeviceName = Environment.MachineName;
        private string DeviceId;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Clear User's Credentials
            StorageHelpers.Instance.DeleteToken(DeviceName);
            this.ContentFrame.Navigate(typeof(LoginPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Store the SdkSettings Locally
            if (!File.Exists(JellyfinSettingsFile))
            {

                // First Run
                DeviceId = Guid.NewGuid().ToString();

                JObject sdkSettings = new JObject(
                    new JProperty(nameof(ClientName), ClientName),
                    new JProperty(nameof(ClientVersion), ClientVersion),
                    new JProperty(nameof(DeviceName), DeviceName),
                    new JProperty(nameof(DeviceId), DeviceId));

                File.WriteAllText(JellyfinSettingsFile, sdkSettings.ToString());
            }

            App.ClientSettingsSdk = new SdkClientSettings();
            App.ClientSettingsSdk.InitializeClientSettings(ClientName, ClientVersion, DeviceName, DeviceId);


            // Get Access Token 
            // If that exists then nav to MainPage
            if (string.IsNullOrEmpty(StorageHelpers.Instance.LoadToken(DeviceName)))
            {
                // if Not then login
                this.ContentFrame.Navigate(typeof(LoginPage));
            } else
            {
                this.ContentFrame.Navigate(typeof(MainPage));
            }

        }
    }
}
