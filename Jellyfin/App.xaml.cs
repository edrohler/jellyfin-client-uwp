using Jellyfin.Sdk;
using Jellyfin.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Common;
using Jellyfin.Helpers;
using Newtonsoft.Json;

namespace Jellyfin
{
    sealed partial class App : Application
    {
        public App()
        {
            SdkClientSettings = ConfigureSdkSettings();
            Services = ConfigureServices();
            
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        
        public new static App Current => (App)Application.Current;

        public SdkClientSettings SdkClientSettings { get; set; }
        
        public IServiceProvider Services { get; }

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
        
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Add Jellyfin SDK services.
            services.AddSingleton<ISystemClient, SystemClient>();
            services.AddSingleton<IUserClient, UserClient>();
            services.AddSingleton<IUserViewsClient, UserViewsClient>();
            services.AddSingleton<IUserLibraryClient, UserLibraryClient>();

            return services.BuildServiceProvider();
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
