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
            BaseItemDto Library = await UserLibraryClientService.Current.UserLibraryClient.GetItemAsync(App.Current.AppUser.Id, LibraryId);
        }
    }
}
