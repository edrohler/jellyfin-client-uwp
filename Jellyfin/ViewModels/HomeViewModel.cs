using System.Collections.Generic;
using System.Threading.Tasks;
using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        Dictionary<string, IReadOnlyList<BaseItemDto>> LatestMedia = new Dictionary<string, IReadOnlyList<BaseItemDto>>();

        public HomeViewModel()
        {
            
        }

        //Load Home Page Content
        public async Task PageReadyAsync()
        {
            foreach (BaseItemDto item in App.Current.UserViews.Items)
            {
                IReadOnlyList<BaseItemDto> LibraryLatestMedia = await UserLibraryClientService.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.Id, item.Id);
                LatestMedia.Add(item.Name, LibraryLatestMedia);
            }
        }
    }
}
