using CommonHelpers.Common;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {

        public string LibraryTitle { get; set; }

        public ObservableCollection<LibraryModel> LibraryItems {get;set;}
        
        public LibraryViewModel()
        {
            LibraryItems = new ObservableCollection<LibraryModel>();
        }

        public async Task PageReadyAsync(Guid LibraryId)
        {

            await LoadLibraryItemsAsync(LibraryId);
        }

        public async Task LoadLibraryItemsAsync(Guid libId)
        {
            LibraryTitle = App.Current.UserViews.Items.Where(i => i.Id == libId).FirstOrDefault().Name;

            // Gets Library Items
            BaseItemDtoQueryResult items = await ItemsClientService.Current.ItemsClient.GetItemsByUserIdAsync(App.Current.AppUser.Id, parentId: libId);

            foreach (var item in items.Items)
            {
                LibraryItems.Add(new LibraryModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    ImageSrc = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{item.Id}/Images/Primary"))
                });
            }
        }
    }

    public class LibraryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public BitmapImage ImageSrc{ get; set; }
    }
}
