using Jellyfin.Sdk;

namespace Jellyfin.Services
{
    public class UserClientService
    {
        #region Singleton Members

        private static UserClientService _current;

        public static UserClientService Current => _current ?? (_current = new UserClientService());

        #endregion

        #region Instance Members

        public UserClient UserClient { get; set; }

        #endregion
    }
}
