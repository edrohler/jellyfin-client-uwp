using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Services
{
    public class ItemsClientService
    {
        #region Singleton Members

        private static ItemsClientService _current;

        public static ItemsClientService Current => _current ?? (_current = new ItemsClientService());

        #endregion

        #region Instance Members

        public ItemsClient ItemsClient {get;set;}

        #endregion
    }
}
