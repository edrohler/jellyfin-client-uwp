using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Threading.Tasks;

namespace Jellyfin.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        public BaseItemDto BaseItem;

        public ItemViewModel()
        {

        }

        public async Task PageReadyAsync(Guid id)
        {
            BaseItem = await JellyfinClientServices.Current.UserLibraryClient.GetItemAsync(App.Current.AppUser.User.Id, id);

            Console.WriteLine();
        }
    }
}
