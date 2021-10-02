using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class ItemPage : Page
    {
        public ItemPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.PageReadyAsync((Guid)e.Parameter);
        }
    }
}
