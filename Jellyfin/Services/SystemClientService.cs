using Jellyfin.Sdk;

namespace Jellyfin.Services
{
    public class SystemClientService
    {
        #region Singleton Members

        private static SystemClientService _current;

        public static SystemClientService Current => _current ?? (_current = new SystemClientService());

        #endregion

        #region Instance Members

        public SystemClient SystemClient { get; set; }

        #endregion
    }
}
