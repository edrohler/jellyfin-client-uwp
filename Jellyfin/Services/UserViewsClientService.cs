using Jellyfin.Sdk;

namespace Jellyfin.Services
{
    public class UserViewsClientService
    {
        #region Singleton Members

        private static UserViewsClientService _current;

        public static UserViewsClientService Current => _current ?? (_current = new UserViewsClientService());

        #endregion

        #region Instance Members

        public UserViewsClient UserViewsClient { get; set; }

        #endregion
    }
}
