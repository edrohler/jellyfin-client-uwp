using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        IReadOnlyList<BaseItemDto> LatestMedia = null;
        

        public HomeViewModel()
        {
            
        }

        //Load Home Page Content
        //Sections:
        //      My Media Tiles to Collections
        //      Continue Watching
        //      Next Up
        //      Latest for each collection
        public async Task PageReadyAsync()
        {
            // Returns a collection of the latest 20 media items
            LatestMedia = await UserLibraryClientService.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.Id);
        }
    }
}
