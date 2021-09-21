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

namespace Jellyfin.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private string searchTerm;

        public ShellViewModel()
        {
            MenuItems = new ObservableCollection<MenuDataItem>();
            SuggestBoxItems = new ObservableCollection<string>();
            
            RefreshCommand = new DelegateCommand(Refresh);

            // This is only so you can see menu items at design time
            if (DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled)
            {
                LoadMenuItemsAsync().ConfigureAwait(false);
                LoadSearchBoxNamesAsync().ConfigureAwait(false);
            }
        }

        public ObservableCollection<MenuDataItem> MenuItems { get; set; }

        public ObservableCollection<string> SuggestBoxItems { get; set; }

        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                // The SetProperty method will return 'true' if the new value is different (anything inside the if block will run if the value changed)
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

        // IMPORTANT: You should never run async code in a constructor. so we signal the view model when the page is ready in this Task
        public async Task PageReadyAsync()
        {
            await LoadMenuItemsAsync();

            await LoadSearchBoxNamesAsync();
        }

        private async Task LoadMenuItemsAsync()
        {
            // START - API simulation code
            // Just simulating getting the user's libraries
            await Task.Delay(1000);

            // This is only to mimic what you'll get from the API ( I doubt groupItem is the correct model). 
            var mediaLibraries = new List<string>()
            {
                "Home",
                "TV",
                "Movies",
                "Music",
                "Photos",
                "Games"
            };

            // END - API simulation code
            
            
            // Now you can iterate over the user's groups/library and create a menu item for it

            // In case this is a refresh
            MenuItems.Clear();

            // Iterate over the API results for the libraries, and create menu item
            foreach (var libraryItem in mediaLibraries)
            {
                var item = new MenuDataItem
                {
                    Name = libraryItem,
                    Icon = IconHelper.GetIconForItem(libraryItem)
                };

                MenuItems.Add(item);
            }
        }

        private async Task LoadSearchBoxNamesAsync()
        {
            // I'm not sure if you want to keep this or not, but it's just how you'd add items to the AutosuggestBox
            SuggestBoxItems.Add("Good Will Hunting");
            SuggestBoxItems.Add("Avengers");
            SuggestBoxItems.Add("when Harry Met Sally");
            SuggestBoxItems.Add("Black Widow");
            SuggestBoxItems.Add("Dune (1977)");
            SuggestBoxItems.Add("dune (2021)");
        }
        
        private void Search()
        {
            // SEARCH with this.SearchTerm
            // (note: you might want research how to add a delay before actually searching with the API)
        }
    }
}
