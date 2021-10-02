using System;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Jellyfin.Common
{
    public static class Constants
    {
        // Constants for SdkClientSettings //

        public static string JellyfinSettingsFile = $"{ApplicationData.Current.LocalFolder.Path}\\Jellyfin-UWP-{Environment.MachineName}.json";

        public static string AppVersion = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

        public static string AppName = Package.Current.DisplayName;

        public static string DeviceName = Environment.MachineName;

        public static Guid DeviceId = Guid.NewGuid();


        // Constants for settings and token names //

        public static string AccessTokenKey = @"EmbyAuthToken";

        public static string PinFilePath = $"{ApplicationData.Current.LocalFolder.Path}\\temporary-pin.txt";
    }
}
