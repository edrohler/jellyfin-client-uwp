using Jellyfin.Common;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
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
            // Set the SdkClientSetting first because everything else requires it
            SdkClientSettings = ConfigureSdkSettings();

            // Setup an HttpClient with Default Headers
            // (every service will reuse the same HttpClient instance)
            DefaultHttpClient = ConfigureDefaultHttpClient();

            // Configure Jellyfin Services
            ConfigureJellyfinServices();

            InitializeComponent();
            Suspending += OnSuspending;
        }

        public new static App Current => (App)Application.Current;

        public SdkClientSettings SdkClientSettings { get; private set; }

        public HttpClient DefaultHttpClient { get; private set; }

        // Jellyfin Global Objects
        public UserDto AppUser { get; set; } = null;
        public BaseItemDtoQueryResult UserViews { get; set; } = null;
        public DeviceIdentification DviceIdentification { get; set; } = null;

        // For access to the shell's navigation Frame from the LoginViewModel
        public ShellPage Shell { get; set; }

        // For navigation outside of the ShellPage
        public Frame RootFrame { get; set; }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            RootFrame = Window.Current.Content as Frame;
            
            if (RootFrame == null)
            {
                RootFrame = new Frame();

                RootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                
                Window.Current.Content = RootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    // Condition 1 - If there is no token stored, go to LoginPage
                    if (string.IsNullOrEmpty(StorageHelpers.Instance.LoadToken(Constants.AccessTokenKey)))
                    {
                        RootFrame.Navigate(typeof(LoginPage));
                    }

                    try
                    {
                        // Condition 2 - If there is a token user has authenticated
                        // Test if it is a good Token by getting Current User.
                        // If 200 retuls, go to ShellPage
                        // Else catch and log the error
                        AppUser = await JellyfinClientServices.Current.UserClient.GetCurrentUserAsync();
                        RootFrame.Navigate(typeof(ShellPage), e.Arguments);
                    }
                    catch (Exception ex)
                    {
                        // Condition 3 - Bad token, clear settings toekn and re-auth.
                        ExceptionLogger.LogException(ex);
                        SdkClientSettings.AccessToken = null;
                        RootFrame.Navigate(typeof(LoginPage));
                    }
                }

                Window.Current.Activate();
            }
        }

        // Initialize SdkClientSettings
        // Access globally using App.Current.ClientSettingsSdk
        private SdkClientSettings ConfigureSdkSettings()
        {
#if DEBUG
            // User for testing Login
            //StorageHelpers.Instance.DeleteToken(Constants.AccessTokenKey);
#endif

            if (File.Exists(Constants.JellyfinSettingsFile))
            {
                // ************************************************************ //
                // Sdk Setting Scenario 1: the app has an existing SdkSettings
                // ************************************************************ //

                // Load the existing file
                JObject json = JObject.Parse(File.ReadAllText(Constants.JellyfinSettingsFile));

                // New up and SdkClientSettings
                SdkClientSettings sdkSettings = new SdkClientSettings();

                // Initialize the SdkClientSettings object
                sdkSettings.InitializeClientSettings(
                    json[nameof(Constants.AppName)].ToString(),
                    json[nameof(Constants.AppVersion)].ToString(),
                    json[nameof(Constants.DeviceName)].ToString(),
                    json[nameof(Constants.DeviceId)].ToString());

                // Set the known BaseUrl if any
                sdkSettings.BaseUrl = json["BaseUrl"].ToString();

                // Checks if AccessToken exists and sets the SDK Settings AccessToken from storage
                // If not, will continue to LoginPage
                if (!string.IsNullOrEmpty(StorageHelpers.Instance.LoadToken(Constants.AccessTokenKey)))
                {
                    sdkSettings.AccessToken = StorageHelpers.Instance.LoadToken(Constants.AccessTokenKey);
                }

                return sdkSettings;
            }
            else
            {
                // ********************************************************************************* //
                // Sdk Setting Scenario: This is the first run (or PC Settings -> Reset App was used)
                // ********************************************************************************* //

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

        private void ConfigureJellyfinServices()
        {
            // Configure SystemClient
            JellyfinClientServices.Current.SystemClient = new SystemClient(
                SdkClientSettings,
                DefaultHttpClient);

            // Configure UserClient
            JellyfinClientServices.Current.UserClient = new UserClient(
                SdkClientSettings,
                DefaultHttpClient);

            // Configure UserLibraryClient
            JellyfinClientServices.Current.UserLibraryClient = new UserLibraryClient(
                SdkClientSettings,
                DefaultHttpClient);

            // Configure UserViewsClient
            JellyfinClientServices.Current.UserViewsClient = new UserViewsClient(
                SdkClientSettings,
                DefaultHttpClient);

            // Configure ItemsClient
            JellyfinClientServices.Current.ItemsClient = new ItemsClient(
                SdkClientSettings,
                DefaultHttpClient);

            // Configure ImagesClient
            JellyfinClientServices.Current.ImageClient = new ImageClient(
                SdkClientSettings,
                DefaultHttpClient);

            //Configure DevicesClient
            JellyfinClientServices.Current.DevicesClient = new DevicesClient(
                SdkClientSettings,
                DefaultHttpClient);

        }
        
        public HttpClient ConfigureDefaultHttpClient()
        {
            // Set automatic decompression
            HttpClientHandler handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // Create an Httpclient using the handler
            HttpClient client = new HttpClient(handler);

            // Set the HttpClient's headers
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(
                    SdkClientSettings.ClientName,
                    SdkClientSettings.ClientVersion));

            // Add Default Headers
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json", 1.0));

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("*/*", 0.8));

            return client;
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
