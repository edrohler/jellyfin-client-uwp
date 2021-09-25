using System;
using System.Collections.Generic;
using CommonHelpers.Common;
using Jellyfin.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using CommonHelpers.Mvvm;
using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Common;
using Jellyfin.Views;
using Jellyfin.Services;

namespace Jellyfin.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private string searchTerm;
        private UserDto userDto = null;

        public ShellViewModel()
        {
            MenuItems = new ObservableCollection<MenuDataItem>();
            SuggestBoxItems = new ObservableCollection<string>();
            
            RefreshCommand = new DelegateCommand(Refresh);

            //// This is only so you can see menu items at design time
            //if (DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled)
            //{
            //    LoadMenuItemsAsync().ConfigureAwait(false);
            //    LoadSearchBoxNamesAsync().ConfigureAwait(false);
            //}
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
                    Search();
                }
            }
        }

        public DelegateCommand RefreshCommand { get; set; }

        private void Refresh()
        {
            
        }

        public async Task PageReadyAsync()
        {
            this.userDto = await UserClientService.Current.UserLibraryClient.GetCurrentUserAsync();

            await LoadMenuItemsAsync(userDto.Id);

            //await LoadSearchBoxNamesAsync();
        }

        private async Task LoadMenuItemsAsync(Guid userId)
        {
            // Get User Collections for Menu
            BaseItemDtoQueryResult userViews = await UserViewsClientService.Current.UserViewsClient.GetUserViewsAsync(userId);

            // In case this is a refresh
            MenuItems.Clear();

            // Add Home Menu Item
            MenuItems.Add(new MenuDataItem
            {
                Name = "Home",
                Icon = IconHelper.GetIconForItem("Home")
            });

            // Iterate over the API results for the libraries, and create additional menu items
            foreach (BaseItemDto libraryItem in userViews.Items)
            {
                MenuDataItem item = new MenuDataItem
                {
                    Name = libraryItem.Name,
                    Icon = IconHelper.GetIconForItem(libraryItem.CollectionType)
                };

                MenuItems.Add(item);
            }
        }

        //private async Task LoadSearchBoxNamesAsync()
        //{
        //    // I'm not sure if you want to keep this or not,
        //    // but it's just how you'd add items to the AutosuggestBox
        //    SuggestBoxItems.Add("Good Will Hunting");
        //    SuggestBoxItems.Add("Avengers");
        //    SuggestBoxItems.Add("when Harry Met Sally");
        //    SuggestBoxItems.Add("Black Widow");
        //    SuggestBoxItems.Add("Dune (1977)");
        //    SuggestBoxItems.Add("dune (2021)");
        //}
        
        private void Search()
        {
            // SEARCH with this.SearchTerm
            // (note: you might want research how to add a delay before actually searching with the API)
        }
    }
}
