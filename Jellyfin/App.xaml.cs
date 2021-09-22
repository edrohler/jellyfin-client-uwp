using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            // We need to setup an HttpClient that has the correct headers
            // (every service will reuse the same HttpClient instance)
            //DefaultHttpClient = ConfigureDefaultHttpClient("");

            // Now, we can setup the services using the previously configured requisites
            ConfigureServices();
            
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        
        public new static App Current => (App)Application.Current;

        public SdkClientSettings SdkClientSettings { get; private set; }

        public HttpClient DefaultHttpClient { get; private set; }

        // For access to the shell's navigation Frame from the LoginViewModel
        // (every other page has direct access to the contentFrame already)
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
                    // Condition 2 - If the user is authenticated, go to ShellPage
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

#if DEBUG
            File.Delete(Constants.JellyfinSettingsFile);
#endif

            if (File.Exists(Constants.JellyfinSettingsFile))
            {
                // ************************************************ //
                // Scenario 1: the user has an existing SdkSettings
                // ************************************************ //

                // Load the existing file
                string json = File.ReadAllText(Constants.JellyfinSettingsFile);

                // Deserialize the json into an SdkClientSettings object
                SdkClientSettings sdkSettings = JsonConvert.DeserializeObject<SdkClientSettings>(json);

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

                SdkClientSettings sdkSettings = new SdkClientSettings();

                // Initialize the SdkClientSettings object
                sdkSettings.InitializeClientSettings(
                    Constants.AppName,
                    Constants.AppVersion,
                    Constants.DeviceName,
                    Constants.DeviceId.ToString());

                // Serialize the values into JSON
                JObject serializedSettings = new JObject(
                    new JProperty(nameof(Constants.AppName), Constants.AppName),
                    new JProperty(nameof(Constants.AppVersion), Constants.AppVersion),
                    new JProperty(nameof(Constants.DeviceName), Constants.DeviceName),
                    new JProperty(nameof(Constants.DeviceId), Constants.DeviceId.ToString()),
                    new JProperty("BaseUrl", ""));

                // Save the file to local storage
                File.WriteAllText(Constants.JellyfinSettingsFile, serializedSettings.ToString());

                return sdkSettings;
            }
        }

        private void ConfigureServices()
        {
            SystemClientService.Current.SystemClient = new SystemClient(
                this.SdkClientSettings,
                this.DefaultHttpClient);

            UserClientService.Current.UserLibraryClient = new UserClient(
                this.SdkClientSettings,
                this.DefaultHttpClient);

            UserLibraryClientService.Current.UserLibraryClient = new UserLibraryClient(
                this.SdkClientSettings,
                this.DefaultHttpClient);

            UserViewsClientService.Current.UserViewsClient = new UserViewsClient(
                this.SdkClientSettings,
                this.DefaultHttpClient);
        }
        
        public void ConfigureDefaultHttpClient(Uri BaseUrl)
        {
            // Setting up automatic decompression
            HttpClientHandler handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // create an Httpclient using the handler
            this.DefaultHttpClient = new HttpClient(handler);

            this.DefaultHttpClient.BaseAddress = BaseUrl;

            // Set the HttpClient's headers
            this.DefaultHttpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(
                    this.SdkClientSettings.ClientName,
                    this.SdkClientSettings.ClientVersion));

            this.DefaultHttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json", 1.0));

            this.DefaultHttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("*/*", 0.8));

            //return client;
        }
        
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

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
