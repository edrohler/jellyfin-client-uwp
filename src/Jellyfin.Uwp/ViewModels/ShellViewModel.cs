using System;
using CommonHelpers.Common;
using Jellyfin.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommonHelpers.Mvvm;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Common;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Jellyfin.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private string searchTerm;

        private SearchClient searchClient = null;

        public ShellViewModel()
        {
            MenuItems = new ObservableCollection<MenuDataItem>();
            SuggestBoxItems = new ObservableCollection<string>();
            
            RefreshCommand = new DelegateCommand(Refresh);
        }

        public ObservableCollection<MenuDataItem> MenuItems { get; set; }

        public ObservableCollection<string> SuggestBoxItems { get; set; }

        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                // The SetProperty method will return 'true' if the new value is different
                // (anything inside the if block will run if the value changed)
                if (SetProperty(ref searchTerm, value))
                {
                    SearchAsync();
                }
            }
        }

        public DelegateCommand RefreshCommand { get; set; }

        private void Refresh()
        {
            
        }

        public async Task PageReadyAsync()
        {
            ICollection<SessionInfo> CurrentSessions = (ICollection<SessionInfo>)await JellyfinClientServices.Current.SessionClient.GetSessionsAsync();

            if (CurrentSessions.Count > 0)
            {
                App.Current.SessionInfo = CurrentSessions.FirstOrDefault(i => i.DeviceId == App.Current.SdkClientSettings.DeviceId);
            } else
            {
                App.Current.SessionInfo = new SessionInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    ApplicationVersion = Constants.AppVersion,
                    Client = SystemInformation.Instance.DeviceFamily,
                    DeviceId = App.Current.SdkClientSettings.DeviceId,
                    DeviceName = App.Current.SdkClientSettings.DeviceName,
                    IsActive = true,
                    DeviceType = SystemInformation.Instance.OperatingSystemArchitecture.ToString()
                };
            }

            // Query Server Info
            App.Current.PublicSystemInfo = await JellyfinClientServices.Current.SystemClient.GetPublicSystemInfoAsync();

            // Set Client Info
            App.Current.DeviceIdentification = new DeviceIdentification
            {
                FriendlyName = App.Current.SdkClientSettings.DeviceName,
                Manufacturer = SystemInformation.Instance.DeviceManufacturer,
                ModelName = SystemInformation.Instance.DeviceModel
            };

            await LoadMenuItemsAsync(App.Current.AppUser.Id);
        }

        private async Task LoadMenuItemsAsync(Guid userId)
        {
            // Get User Collections for Menu
            App.Current.UserViews = await JellyfinClientServices.Current.UserViewsClient.GetUserViewsAsync(userId);

            // In case this is a refresh
            MenuItems.Clear();

            // Add Home Menu Item
            MenuItems.Add(new MenuDataItem
            {
                Name = "Home",
                Icon = IconHelper.GetIconForItem("Home")
            });

            // Iterate over the API results for the libraries, and create additional menu items
            foreach (BaseItemDto libraryItem in App.Current.UserViews.Items)
            {
                MenuDataItem item = new MenuDataItem
                {
                    Name = libraryItem.Name,
                    Icon = IconHelper.GetIconForItem(libraryItem.CollectionType),
                    Id = libraryItem.Id
                };

                MenuItems.Add(item);
            }
        }
        
        private async void SearchAsync()
        {
            // SEARCH with this.SearchTerm
            // Debounce inputs
            if (this.SearchTerm.Length > 3)
            {
                this.searchClient = new SearchClient(App.Current.SdkClientSettings, App.Current.DefaultHttpClient);

                SearchHintResult result = await searchClient.GetAsync(SearchTerm);
                
                // Clear Previous Search Items
                SuggestBoxItems.Clear();

                // Add search hints name and type to suggestions
                foreach (SearchHint item in result.SearchHints)
                {
                    SuggestBoxItems.Add($"{item.Name} - {item.Type}");
                }
            }

        }
    }
}
