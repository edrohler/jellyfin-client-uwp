using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin
{
    sealed partial class App : Application
    {
        public App()
        {
            // We need to set the SdkClientSetting first because everything else requires it
            SdkClientSettings = ConfigureSdkSettings();

            // We need to setup an HttpClient that has the correct headers (every service will reuse the same HttpClient instance)
            DefaultHttpClient = ConfigureDefaultHttpClient();

            // Now, we can setup the services using the previously configured requisites
            ConfigureServices();
            
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        
        public new static App Current => (App)Application.Current;

        public SdkClientSettings SdkClientSettings { get; private set; }

        public HttpClient DefaultHttpClient { get; private set; }

        // For access to the shell's navigation Frame from the LoginViewModel (every other page has direct access to the contentFrame already)
        public ShellPage Shell { get; set; }
        
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Condition 1 - If there is no token stored, go to LoginPage
                    // Condition 1 - If the user is authenticated, go to ShellPage
                    rootFrame.Navigate(string.IsNullOrEmpty(StorageHelpers.Instance.LoadToken(Constants.AccessTokenKey)) 
                        ? typeof(LoginPage)
                        : typeof(ShellPage), e.Arguments);
                }

                Window.Current.Activate();
            }
        }

        // Load SdkClientSettings (you can access any value globally using App.Current.ClientSettingsSdk)
        private SdkClientSettings ConfigureSdkSettings()
        {
            if (File.Exists(Constants.JellyfinSettingsFile))
            {
                // ************************************************ //
                // Scenario 1: the user has an existing SdkSettings
                // ************************************************ //

                // Load the existing file
                var json = File.ReadAllText(Constants.JellyfinSettingsFile);

                // Deserialize the json into an SdkClientSettings object
                var sdkSettings = JsonConvert.DeserializeObject<SdkClientSettings>(json);

                // Initialize the SdkClientSettings object
                sdkSettings?.InitializeClientSettings(
                    sdkSettings.ClientName,
                    sdkSettings.ClientVersion,
                    sdkSettings.DeviceName,
                    sdkSettings.DeviceId);

                return sdkSettings;
            }
            else
            {
                // ************************************************************************ //
                // Scenario 2: This is the first run (or PC Settings -> Reset App was used)
                // ************************************************************************ //

                var sdkSettings = new SdkClientSettings();

                // Note: It's safer to only use NewGuid() here so you don't accidentally regenerate the GUID
                sdkSettings.InitializeClientSettings(
                    Constants.AppName,
                    Constants.AppVersion,
                    Constants.DeviceName,
                    Guid.NewGuid().ToString());
                
                // Serialize the values into JSON
                var serializedSettings = JsonConvert.SerializeObject(sdkSettings);

                // Save the file to local storage
                File.WriteAllText(Constants.JellyfinSettingsFile, serializedSettings);

                return sdkSettings;
            }
        }

        private void ConfigureServices()
        {
            SystemClientService.Current.SystemClient = new SystemClient(
                this.ConfigureSdkSettings(),
                this.DefaultHttpClient);

            UserClientService.Current.UserLibraryClient = new UserClient(
                this.ConfigureSdkSettings(),
                this.DefaultHttpClient);

            UserLibraryClientService.Current.UserLibraryClient = new UserLibraryClient(
                this.ConfigureSdkSettings(),
                this.DefaultHttpClient);

            UserViewsClientService.Current.UserViewsClient = new UserViewsClient(
                this.ConfigureSdkSettings(),
                this.DefaultHttpClient);
        }
        
        private HttpClient ConfigureDefaultHttpClient()
        {
            // Setting up automatic decompression
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // create an Httpclient using the handler
            var client = new HttpClient(handler);

            // Set the HttpClient's headers
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(
                    this.SdkClientSettings.ClientName,
                    this.SdkClientSettings.ClientVersion));

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json", 1.0));

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("*/*", 0.8));

            return client;
        }
        
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            //TODO: Save application state and stop any background activity
            // This is where you need to save any state because the app has been suspended.

            deferral.Complete();
        }
        
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
