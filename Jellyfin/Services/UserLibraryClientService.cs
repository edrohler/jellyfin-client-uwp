using System;
using Jellyfin.Sdk;

namespace Jellyfin.Services
{
    public class UserLibraryClientService
    {
        #region Singleton Members

        private static UserLibraryClientService _current;

        public static UserLibraryClientService Current => _current ?? (_current = new UserLibraryClientService());

        #endregion

        #region Instance Members
        
        public UserLibraryClient UserLibraryClient { get; set; }

        #endregion
    }
}
