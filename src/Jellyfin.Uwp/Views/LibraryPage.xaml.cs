using Jellyfin.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class LibraryPage : Page
    {
        public LibraryPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.PageReadyAsync((Guid)e.Parameter);

            LibNavView.SelectedItem = ViewModel.LibraryPageMenuItems.FirstOrDefault();
        }

        private void LibNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //if (args.SelectedItem is MenuDataItem selectedItem)
            //{
            //    Console.WriteLine();
            //}
        }
    }
}
