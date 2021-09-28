using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        public ItemViewModel()
        {

        }

        public async Task PageReadyAsync(Guid id)
        {
            BaseItemDto Item = await UserLibraryClientService.Current.UserLibraryClient.GetItemAsync(App.Current.AppUser.Id, id);
        }
    }
}
