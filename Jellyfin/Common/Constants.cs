using System;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Jellyfin.Common
{
    public static class Constants
    {
        public static string JellyfinSettingsFile { get; } = $"{ApplicationData.Current.LocalFolder.Path}\\Jellyfin-UWP-{Environment.MachineName}.json";

        public static string AppVersion { get; } = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

        public static string AppName { get; } = Package.Current.DisplayName;

        public static string DeviceName { get; } = Environment.MachineName;
    }
}
