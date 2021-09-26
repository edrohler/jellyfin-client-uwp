using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Threading.Tasks;

namespace Jellyfin.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        
        public LibraryViewModel()
        {

        }

        public async Task PageReadyAsync(Guid LibraryId)
        {
            // Gets Library Items
            BaseItemDtoQueryResult LibraryItems = await ItemsClientService.Current.ItemsClient.GetItemsByUserIdAsync(App.Current.AppUser.Id, parentId: LibraryId);
        }
    }
}
